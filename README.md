# 3cs Bang!  

사용엔진: Unity 2020.3.25f1  
개발기간: 2022.01.01 ~

---
## 목차

+ 게임 설명
+ 백엔드 구성  

---
## 게임 설명  
서부시대 배경으로 펼쳐지는 보안관, 부관, 무법자, 배신자를 추리하여 각자 목표를 달성하면 이기는 보드게임 Bang!을 모티브로한 게임  
2.5D 쿼터뷰 멀티플레이 게임이며, 실시간으로 플레이어가 자유롭게 돌아다니며 미션을 진행하고, 다른 플레이어들을 추리하여 각자 직업에 맞는 목표를 달성하면 이기는 게임

### 재구성  
+ 한 턴(Turn)마다 카드를 지급받는 방식을 개인 미션을 달성하면 아이템을 지급하는 방식으로 사용  


(구현예정)
+ 플레이어가 추리를 할수있게끔 모든 플레이어들이 참여하는 돌발협동미션 구현 예정  
+ 승마 (에셋, 애니메이션) 구매완료, 아직 게임에 미적용  
+ 감옥에 갇혔을때 탈출할 수 있는 특정 아이템을 가지고있을 시 간수에게 말을 걸어 탈출 가능   

---
## 백엔드 

엑시트 게임즈에서 개발한 네트워크 솔루션 Photon Unity Network 사용  
소프트웨어 및 관련 데이터는 중앙에 호스팅되고 사용자는 웹 브라우저 등의 클라이언트를 통해 접속하는 형태인 Saas(Software as a service) Photon Cloud를 사용  

### 사용
+ Photon PUN - (클라이언트/서버)
+ Photon Chat - (채팅 시스템)
+ playFab - (데이터베이스)

### Scene 흐름

![Scene](https://drive.google.com/uc?export=view&id=1rxi3rNfg41JVnqnuS7guwVQw0nhaKJJo)
백엔드에 사용되는 PunCallbacks PunChat PlayFab 파일들은 Singleton으로 선언

### 백엔드 흐름
![PhotonConnect1](https://drive.google.com/uc?export=view&id=1oL-mmjmCgdyl8P8My4MaNHJFgq5-JQHz)
![PhotonConnect2](https://drive.google.com/uc?export=view&id=1anRfcLVgx86pvEYS3RUqlCLzppGlJGea)


