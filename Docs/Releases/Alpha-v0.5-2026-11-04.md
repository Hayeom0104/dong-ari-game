# Alpha v0.5 Windows 빌드 인수인계 — 2026-11-04

## 업로드된 배포 아카이브
- 이름: `DongAriGame_Alpha_v0.5_Windows.zip`
- 종류: Windows x64 실행 빌드 (Unity Editor 프로젝트 아님)
- 파일 크기: 81,587,615 bytes
- SHA-256: `3b553e0439fc0848bf8c147d836b211d3ae159ba166a5a8448b689d076470d83`
- 엔진 버전(동봉 README): Unity 6000.5.10f1
- 실행 경로(압축 해제 후): `Game/DongAriGame.exe`
- 아카이브 검증: ZIP 항목 무결성 테스트 통과

> **중요:** GitHub에 본 Windows ZIP 자체는 업로드되지 않았습니다. 이 커밋은 사용자가 제공한 아카이브의 메타데이터 및 QA 인수인계 기록입니다. 바이너리 업로드를 지원하지 않는 현재 연결 방식 때문에 실행 파일을 이 브랜치의 소스 트리에 추가하지 않았습니다. ZIP을 받으려면 채팅에 업로드된 원본 파일을 별도로 보관하거나 GitHub Releases에 직접 첨부해야 합니다.

## 아카이브 내 README에 기록된 v0.5 특징
- 총 10개 방, 방별 발판 지형과 조명
- 6~8번 방 두 번째 웨이브 양쪽 소환 예고
- 5번 방 중간 보스: 돌진 / 지면 강타
- 10번 방 최종 보스: 룬 폭발 / 조준 마탄 / 체력 절반 이하 3방향 공격
- 직업과 독립된 무기 교체, 6종 무기, J 공격 / Q 스킬 / R 직업 궁극기
- E 액티브 아티팩트 / Shift 회피 대시
- 4·9번 방 이동형 휴식 상점, 상인·대장장이·도박사, F 상호작용
- HP/MP 바, 스킬 UI, 원형 쿨다운, 툴팁
- 저장/이어하기 없음

## 동봉 QA 기록 (자체 보고)
- `QA/result.txt`: PASSED
- `QA/playmode.xml`: 27/27 통과, 실패 0, 스킵 0 (기록 시각: 2026-11-03 UTC)
- `QA/checks.txt`: Windows 진행 검증 항목 전부 PASS (동봉 README에는 66/66으로 기술)
- `QA/Screenshots/`: 캐릭터 선택, 전투, 보상, 상점, 보스, 엔딩 등 이미지 21개
- `Alpha_구현_QA.xlsx`: 구현 및 QA 점검표

주의: 위 결과는 **업로드된 파일에 담긴 기록**이며 이 커밋을 만드는 환경에서 Windows 실행을 새로 검증한 것은 아닙니다.

## GitHub 코드와의 관계
- 원래 `main` 브랜치 기준 커밋: `1d7fc4aa56a8a2cd91094a2d9cf7ad3f57162a61` (2026-10-06 UTC)
- 아카이브에는 `Assets/`, `Packages/`, `ProjectSettings/` 등의 Unity 개발 프로젝트 원본이 없습니다.
- 배포된 v0.5 빌드와 GitHub의 예전 Unity 소스가 동일하다고 가정하거나, 바이너리에서 소스가 복구되었다고 주장하면 안 됩니다.
- **다음 Work 작업:** v0.5에 대응하는 Source ZIP 확보 → 소스 동기화 → Unity 6000.5.10f1에서 컴파일/PlayMode/Windows 빌드 확인 → GitHub 릴리스 바이너리 배포.

## 작업 흐름
- `release/alpha-v0.5-2026-11-04`: 이번 배포본 QA/메타데이터 추적용 브랜치
- `work/alpha-v0.5-next`: 후속 Unity 소스 작업용 별도 브랜치 (연결 예정)
