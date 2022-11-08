# 太吾快捷键框架 & 优化

## 这是什么
这是一个基于太吾绘卷(The Scroll Of Taiwu)的框架及优化Mod

## 功能
为太吾绘卷的Mod添加了注册、检测UI快捷键的功能

新增了一些快捷键功能。

具体功能请参考创意工坊链接：https://steamcommunity.com/sharedfiles/filedetails/?id=2885985836

## 使用该框架

### 快速开始

若要方便的使用该Mod框架，按如下方式快速创建一个快捷键组件类：
```C#
namespace TestShortcutMod
{
    public class MyShortcutCollection : ModShortcutCollection
    {
        public ShortcutGroup ModShortcutGroup = new ShortcutGroup("Mod快捷键组", 100);

        public Shortcut Shortcut1 = new Shortcut("Mod快捷键.测试.快捷键1", "快捷键1",KeyCode.Q);
        public Shortcut Shortcut2 = new Shortcut("Mod快捷键.测试.快捷键2", "快捷键2",KeyCode.W);

        public override void OnInit()
        {
            // 注册快捷键设置，使快捷键设置可以在系统设置里显示
            RegisterShortcut(ModShortcutGroup, Shortcut1);
            RegisterShortcut(ModShortcutGroup, Shortcut2);

            // 将快捷键1注册到Bottom界面中，当Bottom界面显示且接受快捷键时，按下快捷键1则会调用对应方法
            RegisterUIShortcut(UIElement.Bottom, Shortcut1, OnBottomUIShortcut)

            // 注册Bottom界面的Update方法，进行每帧检测
            RegisterUIUpdate(UIElement.Bottom, OnBottomUIUpdate);
        }

        private void OnBottomUIShortcut(UIElement element)
        {
            // 打开阅读界面
            var uiBottom = element.UiBaseAs<UI_Bottom>();
            uiBottom.OnReadingClicked();
        }

        private void OnBottomUIUpdate(UIElement element)
        {
            // 检测快捷键2是否在UI允许监听快捷键的情况下按下
            if(Shortcut2.Check(element))
            {
                // 打开阅读界面
                var uiBottom = element.UiBaseAs<UI_Bottom>();
                uiBottom.OnReadingClicked();
            }
        }
    }
}
```

随后在Mod插件里写上快捷键组件类的初始化、销毁功能
```C#
namespace TestShortcutMod
{
    [PluginConfig("快捷键测试Mod", "作者", "1.0.0")]
    public class ModMain : TaiwuRemakePlugin
    {
        private MyShortcutCollection MyShortcutCollection { get; set; }

        private bool enableShortcut;

        public override void Initialize()
        {
            MyShortcutCollection = new MyShortcutCollection();
            MyShortcutCollection.Init();
        }

        public override void OnModSettingUpdate()
        {
            ModManager.GetSetting(ModIdStr, "Bool_EnableShortcut", ref enableShortcut);
            MyShortcutCollection.SetEnable(enableShortcut);
        }

        public override void Dispose()
        {
            MyShortcutCollection.Dispose();
            MyShortcutCollection = null;
        }
    }
}
```

这样便完成了基本的快捷键功能设置。

### ShortcutManager类
许多快捷键的方法实际上最终于ShortcutManager中调用。若是有需求，可以直接查看该类之中的方法并进行调用。