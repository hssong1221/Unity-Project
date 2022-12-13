# 3cs Bang!  

사용엔진: Unity 2020.3.25f1  
개발기간: 2022.01.01 ~ 04.21, 07.18 ~ 12.02  
게임 플레이 영상: https://www.youtube.com/watch?v=Mkr0GmjKpYc

---
목차
=========================================================================================================================
+ 게임 설명  
+ 백엔드 구성  
  * [사용한 SDK](#사용한-SDK)  
  * [Scene 흐름](#Scene-흐름)  
  * [턴 구현 로직](#턴-구현-로직)  
+ 인게임 구성
  * [게임 매니저](#게임-매니저)
  * [퀘스트](#퀘스트)
  * 상호작용
    - [Npc 상호작용](#Npc-상호작용)  
    - [QuestItem 상호작용](#QuestItem-상호작용)  
    - [Player 상호작용](#Player-상호작용)
      + Bang  
  * [애니메이션 동기화](#애니메이션-동기화)  
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


+ ## 턴 구현 로직
  ![PhotonConnect1](https://user-images.githubusercontent.com/22339258/207252888-ffc76cee-c6a5-4072-83cb-27a7459d6ce7.png)
  
  
  |로그인|룸|
  |:-:|:-:|
  |![로그인](https://user-images.githubusercontent.com/22339258/207252873-eb75cadf-fa9b-4988-b843-96e6e9d91ece.png) | ![룸](https://user-images.githubusercontent.com/22339258/207252880-2ef79285-4571-4a99-bf8b-40cc3dd5630f.png) |
  
  |게임플레이|모바일버전|
  |:-:|:-:|
  |![게임플레이](https://user-images.githubusercontent.com/22339258/207252882-f434c497-f431-485e-8895-1bbfec4ef1e7.png) | <img src = "https://user-images.githubusercontent.com/22339258/207252885-601672c7-cb45-49b2-8091-f8a7e515a134.jpg" width="65%" heigth="65%">   ||
  

---
인게임 구성  
=========================================================================================================================

+ ## 게임 매니저
  ![퀘스트 구성도](https://user-images.githubusercontent.com/22339727/164404141-24495b68-87b5-4906-ae6d-0e2bce779097.png)
  - 마스터 클라이언트가 Npc위치를 정한 뒤, 다른 클라이언트들에게 위치정보를 그대로 보내줌
  - 마스터 클라이언트가 캐릭터 직업, 능력, 체력을 정한 뒤, 다른 클라이언트들에게 정보를 그대로 보내줌

+ ## 퀘스트  

  ![퀘스트 구성도](https://user-images.githubusercontent.com/22339727/160859551-d0bb5162-6c2f-47ed-9bf7-a47739547090.PNG)
  - 퀘스트 데이터를 ScriptableObject 스크립트를 이용하여 저장
  - 퀘스트 진행 중 수정되는 값 (퀘스트의 진행도, Npc대화 등)을 위해 Quest_Obj에 Quest를 담아서 하나의 GameObject로 만듦
  - 만들어진 Quest_Obj는 Npc 컴포넌트에 부착되며, 플레이어가 퀘스트 수락 시, 퀘스트 리스트에 추가됨
  - Quest_Pick_Up_Obj 와 Quest_TransPort_Obj 클래스를 이용하여 다양한 줍기, 운송 퀘스트 생성 가능 

+ ## 상호작용
  상호작용 대상과 OnTriggerEnter 시, (F Input) 입력을 받기 위하여 OnTriggerStay (FixedUpdate) 대신 코루틴(Coroutine)을 사용
  ![OnTriggerEnter](https://user-images.githubusercontent.com/22339727/164404827-9ee23a8e-1b7e-4096-965e-9bbe27e73ee0.png)  
    |Npc 대화|QuestItem|
    |:-:|:-:|
    | <img src ="https://user-images.githubusercontent.com/22339727/164404830-d6dc80c3-4608-47ce-8aed-c4b8445f3fe7.png" width="75%" heigth="75%">  |![](https://user-images.githubusercontent.com/22339727/164404824-a5c137e1-6c7d-4ab2-a9fd-818738b51612.png) |


  * ## Npc 상호작용
    ![Npc 상호작용 흐름](https://user-images.githubusercontent.com/22339727/159644683-921775a5-1421-4037-8174-f4b635412f2b.png)  
    ![Npc 대화](https://user-images.githubusercontent.com/22339727/159643990-4427f59b-004d-42bd-838d-43b42b321c96.gif)  


  * ## QuestItem 상호작용  
    ![QuestItem 상호작용 흐름](https://user-images.githubusercontent.com/22339727/159644700-65a9d7f6-0776-4582-aaef-daa4a6992590.png) 
    |||
    |:-:|:-:|
    |![QuestItem Pick상호작용](https://user-images.githubusercontent.com/22339727/159646259-781ebe61-6de4-4171-9daf-00fc62f9afc3.gif)|![QuestItem Pick상호작용취소](https://user-images.githubusercontent.com/22339727/160849044-50218f5c-3d11-4e80-b363-6ecf8a487e92.gif)|


  * ## Player 상호작용
    ![뱅 흐름도](https://user-images.githubusercontent.com/22339727/162135309-5748dd4e-68d6-4ab3-a089-ac78543ec54a.PNG)  
    |||
    |:-:|---|
    |![RayCastAll](https://user-images.githubusercontent.com/22339727/162968008-2cf94a90-dfa7-4f67-ad86-45cc52a08427.PNG)| ![RayCastAllCode](https://user-images.githubusercontent.com/22339727/162968606-395fe67c-e6a1-4c1d-894a-4c33722e9d45.PNG)  단일 RayCast로는 겹친 콜라이더 모두를 판별불가, RayCastAll을 사용하여 hit중 플레이어가있으면 뱅을 실행하고 return|
    
    |사거리 내|사거리 밖|
    |:-:|:-:|
    |![뱅 사거리 내](https://user-images.githubusercontent.com/22339727/162132393-033c4485-c1e6-49d6-b397-d4dfe295dba5.gif)|![뱅 사거리 밖](https://user-images.githubusercontent.com/22339727/162130209-2028d1d0-e485-4fbd-b72f-68c62bb981be.gif)|


+ ## 시행착오

  * ## 아이템
   - 초기 보드게임 컨셉에 맞게 아이템을 카드로 받을 수 있게 구현, 카드는 DOTween을 이용하여 애니메이션 / 정렬  
   - 하지만 컨셉만 빌려오고 게임이 동작하는 방식과 여러 가지 아이템 종류와 그에 대한 설명을 고려하였을 때 아이템 패널을 따로 만드는 것이 효율적이라 판단하여 팀원과 함께 상의 후 현      재와 같이 변경 
   
    |초기|현재|
    |:-:|:-:|
    |![초기](https://user-images.githubusercontent.com/22339727/174551339-f43c5405-7c53-4e8d-a194-4d1efd1aee5e.png)|![현재](https://user-images.githubusercontent.com/22339727/174551341-2655d68a-3d5d-47f5-aae2-812769e119d4.png)|  
    
    또한 설명이 필요한 캐릭터, 능력 정보와 무기 정보를 인게임 UI화면에 배치하는 것 보다 간단한 중요한 정보(주 아이템, 퀘스트 목록 등)을 배치하는 것이 효율적이라 판단하여 팀원과 함     께 상의 후 현재와 같이 변경  
    
 * ## 포톤 애니메이션 동기화  
   - 포톤에서 제공해주는 클라이언트들의 각 캐릭터들 애니메이션을 맞추어주는 Photon Animator View가 제대로 작동하지 않아 서버/클라이언트 간 통신에 사용되는 RaiseEvent를 사용
     ex) SendPlayAnimationEvent(photonView.ViewID, "IsAiming", "Bool", _isAiming);
     
     ![포톤애니메이션동기화](https://user-images.githubusercontent.com/22339727/174551701-d669094b-8762-411c-80d3-b4498c0fcec7.png)
   - PhotonView ID, 애니메이터 파라미터, 타입, 값을 마스터 클라이언트가 모든 월드에게 뿌려주고 해당 PhotonView ID를 가진 캐릭터가 애니메이션을 실행하여 문제 해결
  
  
 * ## 클라이언트 동기화
   - PhotonNetwork.AutomaticallySyncScene = true;
   - 마스터가 LoadLevel()시 다른 클라이언트 모두 동일한 레벨을 로드 해야함
   - 하지만 PhotonView를 부착한 캐릭터가 생성전이기 때문에 RPCs 방법을 사용할 수가 없어 다른 클라이언트들에게 레벨을 로드한다는 공지를 보내지 못함
   - 당시 CPU I5-6600을 사용하였을 때 3개를 같이 실행하면 게임씬으로 넘어가는데 30초 가량 소요되서 클라이언트 내에서의 이벤트 발생을 막아야 했었음
     ![클라이언트 동기화](https://user-images.githubusercontent.com/22339727/174552498-65ff2f41-363c-4175-aaf7-f6589644c687.png)
   - RaiseEvent를 이용하여 같은 룸에있는 클라이언트들에게 이벤트를 보내줌으로서 해결함
   - 또한 캐릭터에게 직접 전하는 이벤트(공격, 아이템 사용 등)는 PhotonView.RPC를 이용하였고 퀘스트 아이템 생성, Npc 생성 등 캐릭터가 직접적으로 관여되지않는 통신은 RaiseEvent를        이용하여 동기화
---  
