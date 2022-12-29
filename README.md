# 3cs Bang!  

사용엔진: Unity 2020.3.25f1  
개발기간: 2022.01.01 ~ 04.21, 07.18 ~ 12.02  
게임 플레이 영상: https://www.youtube.com/watch?v=Mkr0GmjKpYc  
코드 있는 부분으로 바로가기 : https://github.com/hssong1221/Unity-Project/tree/Song/McCree/Assets/Scenes

---
목차
=========================================================================================================================
+ 게임 설명  
+ 백엔드 구성  
  * [사용한 SDK](#사용한-SDK)  
  * [Scene 흐름](#Scene-흐름)  
+ 인게임 구성
  * [턴 구현 로직](#턴-구현-로직)  
  * 상호작용
    - [기본 공격회피 상호작용](#기본-공격회피-상호작용)  

---
게임 설명  
=========================================================================================================================

- 서부시대 배경으로 펼쳐지는 보안관, 부관, 무법자, 배신자를 추리하여 각자 목표를 달성하면 이기는 보드게임 Bang! 을 모티브로한 게임
- 건물 내부로 들어가 의자에 앉아서 3 - 7명이 보드게임을 시작하게 됨  
- 로그인 – 로비 – 룸 – 게임시작 – 게임 진행 – 게임 끝 의 로직을 따르고 있음  

---
백엔드 
=========================================================================================================================
- Photon Unity Network 사용  
- 소프트웨어 및 관련 데이터는 중앙에 호스팅되고 사용자는 웹 브라우저 등의 클라이언트를 통해 접속하는 형태인 Saas(Software as a service) Photon Cloud를 사용  
  <img src = "https://user-images.githubusercontent.com/22339727/163993367-f2b0261b-b1d1-4b39-9aef-5389975f7295.png" width="50%" height="50%">


+ ## 사용한 SDK
  * Photon PUN - (클라이언트/서버)
  * Photon Chat - (채팅 시스템)


+ ## Scene 흐름
  <img src = "https://user-images.githubusercontent.com/22339258/207252893-25651a84-bbed-421e-b090-5c6ea1296be1.png" width="70%" heigth="70%">  

  
  |로그인|룸|
  |:-:|:-:|
  |![로그인](https://user-images.githubusercontent.com/22339258/207252873-eb75cadf-fa9b-4988-b843-96e6e9d91ece.png) | ![룸](https://user-images.githubusercontent.com/22339258/207252880-2ef79285-4571-4a99-bf8b-40cc3dd5630f.png) |
  
  |게임플레이|모바일버전|
  |:-:|:-:|
  |![게임플레이](https://user-images.githubusercontent.com/22339258/207252882-f434c497-f431-485e-8895-1bbfec4ef1e7.png) | <img src = "https://user-images.githubusercontent.com/22339258/207252885-601672c7-cb45-49b2-8091-f8a7e515a134.jpg" width="65%" heigth="65%">   ||
  

---
인게임 구성  -
=========================================================================================================================

+ ## 턴 구현 로직
  ![PhotonConnect1](https://user-images.githubusercontent.com/22339258/207252888-ffc76cee-c6a5-4072-83cb-27a7459d6ce7.png)
  - 마스터 클라이언트가 게임 시작을 명령한다.
  - 게임 시작이 되고 GameLoop 코루틴이 돌면서 각 클라이언트는 턴에 맞는 행동을 하게 된다.

+ ## 상호작용
  * ## 기본 공격회피 상호작용
    ![Npc 상호작용 흐름](https://user-images.githubusercontent.com/22339258/207872036-cd5f31ac-7975-411e-857c-f6df880db42b.png)  
    - PhotonView RPC를 사용해서 필요한 조건과 변수들을 전송하고 받음
    - HP, 대기상태 변수등 


 
---  
