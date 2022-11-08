return {
    Title = "快捷键框架 & 优化",
    FileId = 2885985836,
    Description = [[为游戏内新增了一系列新快捷键，同时添加了一个方便Mod作者添加快捷键的框架。

所有快捷键均可在系统设置界面设置

* 主界面使用Y、U、I、O、P等快捷键打开人物各项界面
* 人物界面使用Y、U、I、O、P等快捷键切换人物各项界面
* 人物界面使用1、2、3、4、5、6快捷键切换人物子项界面
* 人物界面使用Tab键切换当前观察角色
* 人物界面-物品子界面使用Z、X、C快捷键进入修理、拆解、丢弃界面
* 物品获取界面使用空格键快速关闭
* 送礼界面物品选择使用空格键确认，鼠标右键取消
* 产业经营界面物品选择使用空格键确认
* 更多功能有待开发中

讨论Mod、反馈Bug或催更欢迎加入QQ群：689609241]],
    Author = "剑圣(skyswordkill)",
    Source = 0,
    Cover = "Cover.png",
    DefaultSettings =
    {
        {
            DisplayName = "快捷键优化",
            Description = "开启后，将会优化快捷键的使用体验，并新增一些快捷键",
            Key = "Bool_EnableShortcutOptimize",
            DefaultValue = true,
            SettingType = "Toggle"
        },
    },
    FrontendPlugins =
    {
        [1] = "TaiwuShortcut.dll"
    }
}