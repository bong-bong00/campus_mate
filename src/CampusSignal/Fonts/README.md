# Fonts

디자인 핸드오프가 지정한 폰트는 두 종류다.

| 용도 | 폰트 | 폴백 |
|---|---|---|
| 본문 | Pretendard | Segoe UI → Malgun Gothic |
| 수치·라벨 | IBM Plex Mono | Consolas |

두 폰트 모두 오픈 라이선스지만 바이너리를 저장소에 포함하지 않았다.
직접 받아서 이 폴더에 넣으면 `.csproj` 의 `Resource Include="Fonts\**"` 규칙으로
자동 임베드되고, `Themes/Typography.xaml` 의 `FontFamily` 가 그대로 집어간다.

- Pretendard: https://github.com/orioncactus/pretendard (`Pretendard-Regular.otf` 등)
- IBM Plex Mono: https://github.com/IBM/plex

파일을 넣은 뒤 `Themes/Typography.xaml` 의 `AppFontFamily` / `MonoFontFamily` 를
`pack://application:,,,/Fonts/#Pretendard` 형식으로 바꾸면 임베드 폰트를 쓴다.
넣지 않아도 폴백 체인으로 정상 동작한다.
