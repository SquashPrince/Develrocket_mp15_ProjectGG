# ProjectGG

메타플밍 15기 추석기간 사이드 프로젝트 입니다.  \
플레이어는 무기를 장착해 던전을 탐색하며, 적을 처치해 아이템을 얻고 최종 보스를 처치하여 탈출하는 게임입니다.

## 현재 상태

- 진행 기간: 2026-09-19(토) ~ 2026-09-27(일)
- 발표회: 2026-09-28(월)
- 현재 단계: 개발 완료 (발표 완료)

## 환경

| 항목 | 값 |
|---|---|
| 엔진 | Unity 2022.3 LTS |
| 렌더 파이프라인 | Built-in Render Pipeline |
| 입력 | Input Manager |
| 플랫폼 | Windows |

버전이 다르면 프로젝트가 열리지 않거나 설정이 깨집니다.  \
Unity Hub에서 같은 버전을 설치한 뒤 열어주세요.

## 처음 받았다면

1. GitHub Desktop으로 이 저장소를 clone합니다.
2. Unity Hub에서 `Add` 로 clone한 폴더를 추가하고 엽니다.
3. 아래 참고 사항을 확인하며 게임을 진행합니다.

## 팀

- 김재준 (팀장)
- 김하늘
- 권경민
- 주성재
- 조재환

## 참고 사항

1. 기본 조작  \
![게임 시작 이미지](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/play_start.png)


이동 : `W A S D`   \
회피 : `Space`  \
조준 : `마우스`  \
발사 : `마우스 왼쪽 클릭`  \
상호작용 : `E`  \
무기 교체 : `마우스 휠 업`, `마우스 휠 다운`  \
아이템 사용 : `상단 숫자키 1`, `상단 숫자키 2`, `상단 숫자키 3`

\
2. 아이템  \
아이템은 총 13종류가 있으며 각각 효과는 아래와 같습니다.
- ![체력 회복 아이템](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/item_heal.png) : 체력 회복
- ![보호막 회복 아이템](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/item_pill.png) : 보호막 회복
- ![1회 무적 아이템](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/item_shield.png) : 1회 무적
- ![범위 탄막 제거 아이템](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/item_boom.png) : 범위 탄막 제거
- ![랜덤 효과 아이템](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/item_rand.png) : 랜덤 효과
- ![공격력 버프 아이템](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/buff_atk.png) : 공격력 버프
- ![이동속도 버프 아이템](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/buff_spd.png) : 이동속도 버프
- ![최대 체력 증가 아이템](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/aml_heart.png) : 최대 체력 증가
- ![최대 보호막 증가 아이템](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/aml_barrier.png) : 최대 보호막 증가
- ![기본 공격력 증가 아이템](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/aml_atk.png) : 기본 공격력 증가
- ![기본 이동속도 증가 아이템](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/aml_spd.png) : 기본 이동속도 증가
- ![골드 획득량 증가 아이템](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/aml_gold.png) : 골드 획득량 증가
- ![모든 능력치 증가 아이템](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/aml_rain.png) : 모든 능력치 증가

\
3. 무기  \
무기는 총 5종류가 있으며 각각 효과는 아래와 같습니다.
- ![기본 권총](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/wp_hg.png) : 가장 약한 대미지. 재장전 없이 발사
- ![리볼버](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/wp_ar.png) : 무난한 대미지. 무난한 발사 속도
- ![기관단총](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/wp_smg.png) : 낮은 대미지. 빠른 발사속도
- ![샷건](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/wp_sg.png) : 무난한 대미지. 방사형 공격
- ![캐논](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/wp_canon.png) : 높은 대미지. 폭발형 공격

\
4. 맵 이동  \
![맵 이동 이미지](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/play_door.png)

\
플레이어는 던전별 문으로 다가가 상호작용하여 이동할 수 있습니다.  \
단, 몬스터 방에 입장시 모든 몬스터가 처치되어야 다음 방으로 넘어갈 수 있습니다.

\
5. 보스  \
![보스 이미지](https://github.com/SquashPrince/Develrocket_mp15_ProjectGG/blob/main/Docs/README_Images/play_boss.png)

\
보스는 6가지 패턴을 사용하며, 보스 처치시 희귀 아이템과 클리어 포탈이 나타납니다.  \
현 개발 단계에선 이곳을 클리어로 두었고 추가 개발시 스테이지를 늘리는 기능으로 확장 예정입니다.
