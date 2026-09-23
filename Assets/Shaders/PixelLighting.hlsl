#ifndef PIXEL_LIGHTING_INCLUDED
#define PIXEL_LIGHTING_INCLUDED

void GetMainLight_float(out float3 Direction, out float3 Color)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction = normalize(float3(0.5, 0.5, 0.5));
    Color = float3(1, 1, 1);
#else
    Light mainLight = GetMainLight();
    Direction = mainLight.direction;
    Color = mainLight.color;
#endif
}

#endif