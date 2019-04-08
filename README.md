- ybot@Idle.ybot@IdleAvatar 已经调节妥当，其他 ybot 动作可以复制这个 Avatar 的定义。

- 自定义人物模型已经生成，使用 ybot 的动画。

- 自定义人物模型的站立动画可能要换。

- 基础的移动脚本和控制器脚本雏形生成。

- 场景中需要记录状态而带 url 的物体，其 url 的命名规则：

    ```
    [场景名]_[物件缩写]_[颜色和序号]
    ```

    如场景 "Scene1" 中的黄色颜料球，其 url 表示为 "Scene1_cT_yellow0".

    缩写对应：

    - 有颜色的物体，仅用作交换颜色：cT
    - 有颜色的物体，提供颜色并作为游戏进程里程碑：cTsp
    - 有颜色的门，变换为特定颜色时打开：cD
    - NPC：npc

    有颜色的门，颜色表示为能使其打开的那一种。

- 图层：

    - 9：PlayerCharacter，玩家角色
    - 10：RaycastField，地面、墙壁等玩家足部射线检测物件
    - 11：NoShadow，没有影子的墙壁
    - 12：VcRaycastField，带有 VisibilityChanger 组件的物体所在层

- 场景组件规则：

    - 场景根节点
       - 阳光等方向光
       - Enviroment
           - Static：光照贴图静态物体
           - Items：可活动或可交互的物体、非静态光照和模型
           - 其它组件：其它功能性组件
       - LevelGates：切换场景的触发器和进入场景的锚点
       - 其它组件：调试用临时组件



## 完成功能的组件

- 接口 IphysicsInteract，控制物理互动行为，如踩踏板按钮及受其控制的组件。
- 踩踏板按钮。人物站在上面之后切换动画状态并输出。传递信息借助 IphysicsInteract 接口。
- 增加事件驱动管理器 EventManager 和监听器接口，可控制 UI 和物体动画。
- 事件驱动控制的门。
- 可互动物件在接近时增加 UI 提示，这部分使用事件驱动控制。
- 可互动物件的原理：同一时间只有至多一个在激活的可互动物件，在激活的可互动物件的引用存放在全局单例 GlobalHub 中。UI 提示检测引用是否存在决定显示或不显示互动提示戳。玩家控制器的互动功能直接调用全局单例中的引用 GlobalHub.p_enteract。
- 场景切换组件和管理器，通过 Trigger 触发，指向新的场景。可选是否变换位置和人物朝向角度。场景切换管理器在场景切换时搜索并删除 EventManager 中为空的项。
    - <date = Feb 5> 场景管理器切换场景方法更有效，转换场景时摄像机不会有多余的动作。
- 增加文本显示的组件，可互动物件增加了对话组件策略。
- 改进了 CDoor 门的逻辑判断，可以直接控制开和闭，在场景切换后依然记忆状态。
- 文本组件内容从 Resources/Text/point2TalkNode.json 读取，json 文件应从 csv 文件转换。
- <date = Jan10> 增加了 CameraSeek 类和 VisibilityChanger 类
    - CameraSeek 类作为绑定在相机上的组件，向相机看向的方向从屏幕正中发出射线，模拟玩家的注视动作，与 VisibilityChanger 类联动，可设置注视开始到触发的时间。
    - VisibilityChanger 为改变子物体可见性的组件，必须与触发器共存。初始化时可设置 VisibilityChanger.initVisibility 字段改变初始状态的可见性，运行时改变 VisibilityChanger.IsTrig 属性使可见性从有到无或者从无到有。用于生成”注视之下消失“或者”注视之下出现“的场景物件。
- <date = Jan 23> 增加随机生成的迷宫，和迷宫墙壁模型。迷宫单元数量 16 * 9（宽 * 高）。
    - <date = Feb 12> 迷宫单元数量为 16 * 16，包含一个 6 * 6 的空窗。
- <date = Jan 25> CameraSeek 组件可以同时检测多个物体；VisibilityChanger 组件增加专有图层 VcRaycastField，提升检测效率；相机组件增加了将位置转向目标背后的功能。
- <date = Jan 28> 
    - 增加可互动物件 InteractObject 类的新策略 ColorSpring，无颜色的玩家角色靠近时可以互动，为玩家提供特定种类的颜色。
    - 修改可互动物体的接口和策略类，互动函数返回值从 void 改为 int，唤醒可互动物体时引用从策略类改为可互动物件自身，便于增加互动成功或失败的提示音效。
    - 增加流程设计，玩家通过初始房间后进入街市场景，可以渐进开启关卡。最初的关卡没有颜色，获得一种颜色后可以开启后续关卡，后续关卡包含已找到的颜色。随着挑战深入，关卡的颜色更丰富。
- <date = Feb 5> 增加 BuildChanger 组件用于 trick 效果。此组件继承接口 IphysicsInteract，绑定一个碰撞盒，响应从蓝轴指向方向来的触发器事件，玩家仅能从碰撞盒的一面进出使组件切换状态。组件维护建筑物引用数组和对应建筑的可见性数组，状态切换改变建筑物可见性。
- <date = Feb 10> 改进随机迷宫组件，现在可以指定迷宫中的“空穴”，在迷宫空腔位置放入目标点或其他建筑。
- <date = Feb 12> 为随机迷宫增加路径提示组件，在迷宫内指定一处终点，可以得到其他节点与该点的距离（表示为曼哈顿距离）。用于寻路提示。
- <date = Feb25>
    - 增加颜色渐变组件 ColorGradient，可令带有材质或灯光的物体的颜色渐变至目标色。
    - ColorSpring 组件由事件驱动，仅提供一次某种颜色。触发动作作为游戏进度中的里程碑。 
    - 与物体互动的音效使用事件驱动，param：成功 = "Succeed"，失败 = "Fail"，无在活动的互动物体 = "None"。播放音效的组件未添加。
- <date = Feb27>
    - 与 ColorSpring 组件互动后发送互动消息。需要确保每个 ColorSpring 的 Point 属性值唯一。
    - 存档文件相关：GlobalHub.Instance.Url2Point["BKeyFlag"]。按位存放对应 Point 值的 ColorSpring 已触摸的消息。
    - 触摸红色的 ColorSpring 后的转场效果为 trick，需要保证场景 Level_white_0 与 Level_white_1 中的塔顶建筑世界坐标与旋转角一致。
- <date = Mar1>
    - 增加检查游戏事件开关通过后消去场景内某些组件的脚本 DisItemWithGFlag。
    - 迷宫内提示路径的组件进一步完善，等待加入音效。
        - <date = 12> 迷宫内提示音效加入
- <date = Mar25>
    - 加入新的关卡单元
    - 加入针对 GameObject 的对象池，考虑加入泛用对象池。
- <date = Apr1>
    - SceneGate 组件及场景节点 "LevelGates" 下的物件应设置层为 "Ignore Raycast"
- <date = Apr8>
    - 提取 UI 中的显示文字至配置表 @"Resource\Text\uiTexts" 。默认使用简体汉字，未来可加入本地化设置。
        - 所有键值对：
            - newGame：开始游戏
            - newGameNote：开始新的进度并覆盖旧的进度
            - loadGame：继续游戏
            - loadGameNote：从上次结束的位置继续
            - quitGame：退出游戏
            - quitGameNote：退出并关闭游戏
            - colorEnteract：交换颜色
            - colorSource：触摸色彩源
            - check：检查
            - talk：交谈
        - 上述键名也使用在脚本中。
    - 增加存取档组件 SerializeTool，作为静态类。函数在 GlobalHub 单例中调用。
        - SerializeTool.ToFile() 将游戏进度信息序列化至指定文件，如果文件或路径不存在就新建一份。这将覆盖已有的存档文件。
        - SerializeTool.ToObj() 读取序列化文件至内存，没有安全校验。
        - SerializeTool.SaveFileExist() 查询指定路径文件是否存在。
    - 存取档组件集成在 GlobalHub 单例，便于调用及更新游戏进度数据。外部直接通过 GlobalHub.Instance 调用新建、读、写存档指令。
    - 游戏使用自动保存机制，切换场景及与 InteractObject 实例互动时会保存进度。
    - 存档目前使用自定义脚本格式，保存内容为场景名称、位置、朝向和来自 GlobalHub.Instance.Url2Point 的缓存信息。
    - 增加欢迎界面场景 WelcomeScene，作为游戏入口和新建或覆盖、读取游戏进度指令的调用者。鼠标进入按钮范围内显示说明文字，通过事件驱动组件实现。

## TODO

- 建立实验性场景。<接近完成，date = Jan10> <至“找到红色关卡”前，date = Feb27>
    - 单元进度：关卡 无色 -> 红色 待补完。
- 增加序列化/反序列化组件。优先级为：文本组件 >= 带颜色的物件 url 对应颜色枚举 >> 存盘功能
- 预备增加的初始反序列化信息：对唯一 Url 物体在文本中指定预置 Point 属性
- 细化 NPC 控制器的内容