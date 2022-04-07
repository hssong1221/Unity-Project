# 3cs Bang!  

사용엔진: Unity 2020.3.25f1  
개발기간: 2022.01.01 ~

---
목차
=========================================================================================================================
+ 게임 설명
+ 백엔드 구성  
  * [사용한 SDK](#사용한-SDK)  
  * [Scene 흐름](#Scene-흐름)  
  * [백엔드 흐름](#백엔드-흐름)  
+ 인게임 구성
  * [퀘스트](#퀘스트)
  * 상호작용
    - [Npc 상호작용](#Npc-상호작용)  
    - [QuestItem 상호작용](#QuestItem-상호작용)  
    - [Player 상호작용](#Player-상호작용)
      + Bang

---
게임 설명  
=========================================================================================================================
서부시대 배경으로 펼쳐지는 보안관, 부관, 무법자, 배신자를 추리하여 각자 목표를 달성하면 이기는 보드게임 Bang!을 모티브로한 게임  
2.5D 쿼터뷰 멀티플레이 게임이며, 실시간으로 플레이어가 자유롭게 돌아다니며 미션을 진행하고, 다른 플레이어들을 추리하여 각자 직업에 맞는 목표를 달성하면 이기는 게임

+ ## 재구성  
  + 한 턴(Turn)마다 카드를 지급받는 방식을 개인 미션을 달성하면 아이템을 지급하는 방식으로 사용  


+ ## (구현예정)
  + 플레이어가 추리를 할수있게끔 모든 플레이어들이 참여하는 돌발협동미션 구현 예정  
  + 승마 (에셋, 애니메이션) 구매완료, 아직 게임에 미적용  
  + 감옥에 갇혔을때 탈출할 수 있는 특정 아이템을 가지고있을 시 간수에게 말을 걸어 탈출 가능   

---
백엔드 
=========================================================================================================================
엑시트 게임즈에서 개발한 네트워크 솔루션 Photon Unity Network 사용  
소프트웨어 및 관련 데이터는 중앙에 호스팅되고 사용자는 웹 브라우저 등의 클라이언트를 통해 접속하는 형태인 Saas(Software as a service) Photon Cloud를 사용  

+ ## 사용한 SDK
  * Photon PUN - (클라이언트/서버)
  * Photon Chat - (채팅 시스템)
  * playFab - (데이터베이스)


+ ## Scene 흐름
  ![Scene](https://user-images.githubusercontent.com/22339727/159645968-c2dbe87d-9702-40c5-9f3b-4857777bc3c9.jpeg)
  백엔드에 사용되는 PunCallbacks PunChat PlayFab 파일들은 Singleton으로 선언

+ ## 백엔드 흐름
  ![PhotonConnect1](https://user-images.githubusercontent.com/22339727/159645959-60d4f109-49c6-41b9-b9de-d7fc9399d3e4.jpg)
  ![PhotonConnect2](https://user-images.githubusercontent.com/22339727/159645964-9905a2bf-f7de-45e0-b44e-4373e2dbaba5.jpg)

---
인게임 구성  
=========================================================================================================================
+ ## 퀘스트  
  퀘스트의 데이터 원본이 저장되어있는 Quest를  
  퀘스트 도중 바뀌는 값 (퀘스트의 진행도, Npc대화 등) Quest_Obj에 담아서 하나의 GameObject로 만든다.  
  만들어진 Quest_Obj는 Npc GameObject에 들어가게되며 플레이어가 퀘스트 수락 시, 리스트에 추가된다.  
  ![퀘스트 구성도](https://user-images.githubusercontent.com/22339727/160859551-d0bb5162-6c2f-47ed-9bf7-a47739547090.PNG)

+ ## 상호작용
  상호작용 F Input입력을 받기위하여 OnTriggerStay(FixedUpdate)대신 코루틴을 이용  


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
    |사거리 내|사거리 밖|
    |:-:|:-:|
    |![뱅 사거리 내](https://user-images.githubusercontent.com/22339727/162132393-033c4485-c1e6-49d6-b397-d4dfe295dba5.gif)|![뱅 사거리 밖](https://user-images.githubusercontent.com/22339727/162130209-2028d1d0-e485-4fbd-b72f-68c62bb981be.gif)|

---  
