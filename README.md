# Unity_Train_TankBattle

## Unity练手项目 Unity 经典项目Tank Battle改，加入PVE和PVP online功能

### Photon 网址：<https://dashboard.photonengine.com/>

#### Unity版本号：2019.2.3f1

##### 插件：PlayMaker、Photon PUN2 、Fungus

连接Photon服务器，需要在 Others -> Photon --> PhotonUnityNetworking --> Resources --> PhotonServerSettings.asset 中的Settings下APP Id Realtime 填写申请的id号

中国区id处理：<https://vibrantlink.com/chinacloudpun/>

反应堆参考文献：<https://zhuanlan.zhihu.com/p/38359628>

主要文件目录:

- RainbowTower：Unity项目文件夹
  - Assets:资源
    - _Completed：  Tank Battle成品（Unity 官方）
    - Animation:   动画（UI动画）
    - AudioClips：  音频
    - Characters:   角色
    - Fungus：  Fungus插件
    - Fungus Example:  Fungus Demo
    - Scripts：  游戏、联机模块脚本
      - Camera：  相机
      - Common：  通用脚本
      - Helper：  辅助工具
      - Prop：  道具脚本
      - UI：   UI脚本
    - Settings：  继承自ScriptsObject的配置文件
    - Tank：  游戏子模块（local pvp）
    - Tank AI：  游戏子模块（pve）
      - EnemyTanks:  敌机
        - ScriptableObject：资源文件（状态机、属性资源文件）
