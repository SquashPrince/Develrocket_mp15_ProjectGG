using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// Temporary, removed after the migration has been verified.
[InitializeOnLoad]
public static class CodexUsedAssetsJob
{
    const string Work = "C:/Users/wowns/Documents/Codex/2026-09-25/new-chat/work/used-assets/";
    [Serializable] public class Entry { public string path; public string guid; public string[] dependencies; }
    [Serializable] public class Report { public string project; public string error; public bool dirtyScenes; public Entry[] entries; }
    static CodexUsedAssetsJob() { EditorApplication.update += Tick; }
    static void Tick()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode) return;
        string command = Work + "command.txt";
        if (!File.Exists(command)) return;
        string action = File.ReadAllText(command).Trim();
        File.Delete(command);
        try
        {
            if (action == "audit") Audit();
            else if (action == "move") Move();
            else if (action == "verify") Verify();
        }
        catch (Exception ex) { File.WriteAllText(Work + "error.txt", ex.ToString()); }
    }
    static bool DirtyScenes()
    {
        for (int i=0; i<SceneManager.sceneCount; i++)
            if (SceneManager.GetSceneAt(i).isDirty) return true;
        return false;
    }
    static void Audit()
    {
        var all = AssetDatabase.GetAllAssetPaths().Where(p=>p.StartsWith("Assets/") && !AssetDatabase.IsValidFolder(p)).ToArray();
        var report = new Report { project=Application.dataPath, dirtyScenes=DirtyScenes(),
            entries=all.Select(p=>new Entry {path=p, guid=AssetDatabase.AssetPathToGUID(p), dependencies=AssetDatabase.GetDependencies(p,true)}).ToArray() };
        File.WriteAllText(Work + "unity-audit.json", JsonUtility.ToJson(report,true));
    }
    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent=Path.GetDirectoryName(path).Replace('\\','/');
        EnsureFolder(parent);
        if (string.IsNullOrEmpty(AssetDatabase.CreateFolder(parent,Path.GetFileName(path)))) throw new Exception("Cannot create " + path);
    }
    static void Move()
    {
        if (DirtyScenes()) throw new Exception("Save the open scene before migration; no assets moved.");
        string[] paths=File.ReadAllLines(Work+"move-paths.txt").Where(p=>p.Length>0).ToArray();
        foreach(var p in paths)
        {
            if (!p.StartsWith("Assets/Imports/") || p.Contains("..") || !File.Exists(p)) throw new Exception("Invalid source: "+p);
            if (File.Exists(p.Replace("Assets/Imports/","Assets/UsedAssets/"))) throw new Exception("Destination already exists: "+p);
        }
        var moved=new List<string>();
        AssetDatabase.StartAssetEditing();
        try
        {
            foreach(var p in paths)
            {
                string dst=p.Replace("Assets/Imports/","Assets/UsedAssets/");
                EnsureFolder(Path.GetDirectoryName(dst).Replace('\\','/'));
                string error=AssetDatabase.MoveAsset(p,dst);
                if (!string.IsNullOrEmpty(error)) throw new Exception(p+": "+error);
                moved.Add(p);
                File.WriteAllLines(Work+"moved-paths.txt",moved);
            }
        }
        catch
        {
            foreach(var p in moved.AsEnumerable().Reverse())
            {
                string error=AssetDatabase.MoveAsset(p.Replace("Assets/Imports/","Assets/UsedAssets/"),p);
                if (!string.IsNullOrEmpty(error)) File.AppendAllText(Work+"rollback-errors.txt",p+": "+error+"\n");
            }
            throw;
        }
        finally { AssetDatabase.StopAssetEditing(); }
        File.WriteAllText(Work+"move-complete.txt",paths.Length.ToString());
        File.WriteAllText(Work+"command.txt","verify");
    }
    static void Verify()
    {
        var baseline=JsonUtility.FromJson<Report>(File.ReadAllText(Work+"unity-audit.json"));
        var moved=new HashSet<string>(File.ReadAllLines(Work+"move-paths.txt"));
        var failures=new List<string>();
        Func<string,string> remap=p=>moved.Contains(p)?p.Replace("Assets/Imports/","Assets/UsedAssets/"):p;
        foreach(var e in baseline.entries)
        {
            string p=remap(e.path);
            if (AssetDatabase.AssetPathToGUID(p)!=e.guid) failures.Add("GUID changed: "+p);
            var actual=new HashSet<string>(AssetDatabase.GetDependencies(p,true));
            foreach(var dep in e.dependencies)
                if(!actual.Contains(remap(dep))) failures.Add("Dependency missing: "+p+" -> "+remap(dep));
            if(!p.StartsWith("Assets/Imports/"))
                foreach(var dep in actual.Where(d=>d.StartsWith("Assets/Imports/"))) failures.Add("Still depends on Imports: "+p+" -> "+dep);
        }
        File.WriteAllText(Work+"verify.txt",failures.Count==0?"PASS: GUIDs and Unity dependency graph preserved for all assets.":string.Join("\n",failures));
    }
}
