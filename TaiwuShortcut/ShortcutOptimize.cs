using System;
using System.Collections.Generic;
using FrameWork;
using HarmonyLib;
using UnityEngine;

namespace TaiwuShortcut
{
    public class ShortcutOptimize : ModShortcutCollection
    {
        public ShortcutGroup MainViewGroup = new ShortcutGroup("主界面", 5);
        
        public Shortcut ShortcutInfo = new Shortcut("快捷键优化.主界面.人物", "人物界面",KeyCode.Y);
        public Shortcut ShortcutTeam = new Shortcut("快捷键优化.主界面.同道", "同道界面",KeyCode.U);
        public Shortcut ShortcutEquip = new Shortcut("快捷键优化.主界面.装备", "装备界面",KeyCode.I);
        public Shortcut ShortcutItems = new Shortcut("快捷键优化.主界面.持有", "持有界面",KeyCode.O);
        public Shortcut ShortcutLifeSkill = new Shortcut("快捷键优化.主界面.技艺", "技艺界面",KeyCode.P);
        public Shortcut ShortcutCombatSkill = new Shortcut("快捷键优化.主界面.武学", "武学界面",KeyCode.G);
        public Shortcut ShortcutEquipCombatSkill = new Shortcut("快捷键优化.主界面.运功", "运功界面",KeyCode.H);
        public Shortcut ShortcutRelationShip = new Shortcut("快捷键优化.主界面.关系", "关系界面",KeyCode.J);
        public Shortcut ShortcutLifeRecords = new Shortcut("快捷键优化.主界面.经历", "经历界面",KeyCode.K);
        public Shortcut ShortcutInformation = new Shortcut("快捷键优化.主界面.见闻", "见闻界面",KeyCode.L);

        public Shortcut ShortcutSubPage1 = new Shortcut("快捷键优化.主界面.子界面1", "子界面1",KeyCode.Alpha1);
        public Shortcut ShortcutSubPage2 = new Shortcut("快捷键优化.主界面.子界面2", "子界面2",KeyCode.Alpha2);
        public Shortcut ShortcutSubPage3 = new Shortcut("快捷键优化.主界面.子界面3", "子界面3",KeyCode.Alpha3);
        public Shortcut ShortcutSubPage4 = new Shortcut("快捷键优化.主界面.子界面4", "子界面4",KeyCode.Alpha4);
        public Shortcut ShortcutSubPage5 = new Shortcut("快捷键优化.主界面.子界面5", "子界面5",KeyCode.Alpha5);
        public Shortcut ShortcutSubPage6 = new Shortcut("快捷键优化.主界面.子界面6", "子界面6",KeyCode.Alpha6);

        public Shortcut ShortcutSwitchCharacter = new Shortcut("快捷键优化.主界面.切换查看角色", "切换查看角色", KeyCode.Tab);
        
        public Shortcut ShortcutItemsOperationRepair = new Shortcut("快捷键优化.物品界面.批量操作.修理", "物品界面-修理", KeyCode.Z);
        public Shortcut ShortcutItemsOperationDisassemble = new Shortcut("快捷键优化.物品界面.批量操作.拆解", "物品界面-拆解", KeyCode.X);
        public Shortcut ShortcutItemsOperationDiscard = new Shortcut("快捷键优化.物品界面.批量操作.丢弃", "物品界面-丢弃", KeyCode.C);

        public override void OnInit()
        {
            RegisterShortcut(MainViewGroup, ShortcutInfo);
            RegisterShortcut(MainViewGroup, ShortcutTeam);
            RegisterShortcut(MainViewGroup, ShortcutEquip);
            RegisterShortcut(MainViewGroup, ShortcutItems);
            RegisterShortcut(MainViewGroup, ShortcutLifeSkill);
            RegisterShortcut(MainViewGroup, ShortcutCombatSkill);
            RegisterShortcut(MainViewGroup, ShortcutEquipCombatSkill);
            RegisterShortcut(MainViewGroup, ShortcutRelationShip);
            RegisterShortcut(MainViewGroup, ShortcutLifeRecords);
            RegisterShortcut(MainViewGroup, ShortcutInformation);
            
            RegisterShortcut(MainViewGroup, ShortcutSubPage1);
            RegisterShortcut(MainViewGroup, ShortcutSubPage2);
            RegisterShortcut(MainViewGroup, ShortcutSubPage3);
            RegisterShortcut(MainViewGroup, ShortcutSubPage4);
            RegisterShortcut(MainViewGroup, ShortcutSubPage5);
            RegisterShortcut(MainViewGroup, ShortcutSubPage6);
            
            RegisterShortcut(MainViewGroup, ShortcutSwitchCharacter);
            
            RegisterShortcut(MainViewGroup, ShortcutItemsOperationRepair);
            RegisterShortcut(MainViewGroup, ShortcutItemsOperationDisassemble);
            RegisterShortcut(MainViewGroup, ShortcutItemsOperationDiscard);
            
            RegisterUIUpdate(UIElement.Bottom, OnBottomUIUpdate);
            RegisterUIUpdate(UIElement.CharacterMenu, OnCharacterMenuUIUpdate);
            RegisterUIUpdate(UIElement.GetItem, OnGetItemUIUpdate);
            RegisterUIUpdate(UIElement.EventWindow, OnEventWindowUIUpdate);
            RegisterUIUpdate(UIElement.MultiSelectItem, OnMultiSelectItemUIUpdate);
            RegisterUIUpdate(UIElement.CharacterMenu, OnCharacterMenuItemsUIUpdate);
            RegisterUIUpdate(UIElement.ItemMultiplyOperation, OnItemMultiplyOperationUIUpdate);
        }

        private void OnBottomUIUpdate(UIElement element)
        {
            int switchIndex = CheckSelectCharacterMenu(element);
            
            if (switchIndex >= 0)
            {
                var uiBottom = element.UiBaseAs<UI_Bottom>();
                ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
                argBox.Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
                argBox.Set("IsTaiwuTeam", true);
                argBox.Set("CanOperate", Traverse.Create(uiBottom)
                    .Field("_forcedCanOperateInCharacterMenu").GetValue<bool>());
                UIElement.CharacterMenu.SetOnInitArgs(argBox);
                UIManager.Instance.ShowUI(UIElement.CharacterMenu);

                SwitchCharacterMenu(switchIndex);
            }
        }
        
        private void OnCharacterMenuUIUpdate(UIElement element)
        {
            int switchIndex = CheckSelectCharacterMenu(element);
            
            if (switchIndex >= 0)
            {
                SwitchCharacterMenu(switchIndex);
                return;
            }

            int switchSubIndex = CheckSelectCharacterMenuSubPage(element);
            if (switchSubIndex >= 0)
            {
                SwitchCharacterMenuSubPage(switchSubIndex);
            }

            if (ShortcutSwitchCharacter.Check(element))
            {
                SwitchCurrentCharacter();
            }
        }

        private void OnGetItemUIUpdate(UIElement element)
        {
            if (CommonCommandKit.Space.Check(element))
            {
                var uiGetItem = element.UiBaseAs<UI_GetItem>();
                uiGetItem.QuickHide();
            }
        }

        private void OnEventWindowUIUpdate(UIElement element)
        {
            if (CommonCommandKit.Space.Check(element))
            {
                var ui = element.UiBaseAs<UI_EventWindow>();
                var btn = ui.CGet<CButton>("Confirm");
                if (btn.CanClick())
                    btn.onClick.Invoke();
            }
            else if(CommonCommandKit.RightMouse.Check(element))
            {
                var ui = element.UiBaseAs<UI_EventWindow>();
                var obj = ui.LGet("Cancel") as GameObject;
                //Debug.Log(obj);
                var btn = obj.GetComponent<CButton>();
                if (btn.CanClick())
                    btn.onClick.Invoke();
            }
        }
        
        private void OnMultiSelectItemUIUpdate(UIElement element)
        {
            if (CommonCommandKit.Space.Check(element))
            {
                var ui = element.UiBaseAs<UI_MultiSelectItem>();
                var window = ui.CGet<PopupWindow>("PopupWindowBase");
                var btn = window.ConfirmButton;
                if (btn.CanClick())
                    btn.onClick.Invoke();
            }
            else if(CommonCommandKit.RightMouse.Check(element))
            {
                var ui = element.UiBaseAs<UI_MultiSelectItem>();
                var window = ui.CGet<PopupWindow>("PopupWindowBase");
                var btn = window.CancelButton;
                if (btn.CanClick())
                    btn.onClick.Invoke();
            }
        }
        
        private void OnCharacterMenuItemsUIUpdate(UIElement element)
        {
            var subElement = UIElement.CharacterMenuItems;
            if (!(subElement is CharacterMenuSubPageElement subPageElement))
            {
                return;
            }

            if (!subPageElement.Visible)
            {
                return;
            }
            
            if (!UIManager.Instance.IsFocusElement(UIElement.CharacterMenu) || UI_CharacterMenuItems.CurTabIndex != 0)
                return;
            
            if (ShortcutItemsOperationRepair.Check())
            {
                ShowItemOperation(subElement, 0);
            }
            else if (ShortcutItemsOperationDisassemble.Check())
            {
                ShowItemOperation(subElement, 1);
            }
            else if (ShortcutItemsOperationDiscard.Check())
            {
                ShowItemOperation(subElement, 2);
            }
        }
        
        private void ShowItemOperation(UIElement element, int tagIndex)
        {
            var ui = element.UiBaseAs<UI_CharacterMenuItems>();
            var btn = ui.CGet<Refers>("ItemPage").CGet<CButton>("BtnMultiplySelect");
            if (btn.CanClick())
            {
                btn.onClick.Invoke();
                SwitchItemOperation(tagIndex);
            }
        }
        
        private void OnItemMultiplyOperationUIUpdate(UIElement element)
        {
            if (ShortcutItemsOperationRepair.Check(element))
            {
                SwitchItemOperation(0);
            }
            else if (ShortcutItemsOperationDisassemble.Check(element))
            {
                SwitchItemOperation(1);
            }
            else if (ShortcutItemsOperationDiscard.Check(element))
            {
                SwitchItemOperation(2);
            }
        }

        private void SwitchItemOperation(int index)
        {
            var ui = UIElement.ItemMultiplyOperation.UiBaseAs<UI_ItemMultiplyOperation>();
            var tglGroup = ui.CGet<CToggleGroup>("TogGroup");
            var toggle = tglGroup.Get(index);
            
            if(toggle.CanToggle())
                tglGroup.Set(index);
        }

        private int CheckSelectCharacterMenu(UIElement element)
        {
            var selectIndex = -1;
            if (ShortcutInfo.Check(element))
                selectIndex = 0;
            else if (ShortcutTeam.Check(element))
                selectIndex = 1;
            else if (ShortcutEquip.Check(element))
                selectIndex = 2;
            else if (ShortcutItems.Check(element))
                selectIndex = 3;
            else if (ShortcutLifeSkill.Check(element))
                selectIndex = 4;
            else if (ShortcutCombatSkill.Check(element))
                selectIndex = 5;
            else if (ShortcutEquipCombatSkill.Check(element))
                selectIndex = 6;
            else if (ShortcutRelationShip.Check(element))
                selectIndex = 7;
            else if (ShortcutLifeRecords.Check(element))
                selectIndex = 8;
            else if (ShortcutInformation.Check(element))
                selectIndex = 9;
            
            return selectIndex;
        }

        private int CheckSelectCharacterMenuSubPage(UIElement element)
        {
            var selectIndex = -1;
            if (ShortcutSubPage1.Check(element))
                selectIndex = 0;
            else if (ShortcutSubPage2.Check(element))
                selectIndex = 1;
            else if (ShortcutSubPage3.Check(element))
                selectIndex = 2;
            else if (ShortcutSubPage4.Check(element))
                selectIndex = 3;
            else if (ShortcutSubPage5.Check(element))
                selectIndex = 4;
            else if (ShortcutSubPage6.Check(element))
                selectIndex = 5;
            
            return selectIndex;
        }

        private void SwitchCharacterMenu(int switchIndex)
        {
            var uiCharacter = UIElement.CharacterMenu.UiBaseAs<UI_CharacterMenu>();
            var tabTogGroup = Traverse.Create(uiCharacter).Field("_tabTogGroup").GetValue<CToggleGroup>();
            var toggle = tabTogGroup.Get(switchIndex);
            
            if(toggle.CanToggle())
            {
                tabTogGroup.Set(switchIndex);
                var togRefers = toggle.GetComponent<Refers>();
                var highLight = togRefers.CGet<GameObject>("Highlight");
                highLight.SetActive(toggle.isOn);
            }
        }
        
        private void SwitchCharacterMenuSubPage(int switchSubIndex)
        {
            var uiCharacter = UIElement.CharacterMenu.UiBaseAs<UI_CharacterMenu>();
            var subTogGroup = Traverse.Create(uiCharacter).Field("_subTogGroup").GetValue<CToggleGroup>();
            var toggle = subTogGroup.Get(switchSubIndex);
            
            if(toggle.CanToggle())
                subTogGroup.Set(switchSubIndex);
        }

        private void SwitchCurrentCharacter()
        {
            var uiCharacter = UIElement.CharacterMenu.UiBaseAs<UI_CharacterMenu>();
            var curCharIndex = Traverse.Create(uiCharacter).Field<int>("_curCharacterIdIndex").Value;
            var characterIdList = Traverse.Create(uiCharacter).Field<List<int>>("CharacterIdList").Value;
            if(characterIdList.Count < 0)
                return;
            
            var index = (curCharIndex + 1) % characterIdList.Count;
            uiCharacter.SelectCharacter(characterIdList[index]);
            //uiCharacter.RefreshSubPage();
            AccessTools.Method(typeof(UI_CharacterMenu), "RefreshSubPage").Invoke(uiCharacter, 
                Array.Empty<object>());

            if (UIElement.CharacterMenuTeam is CharacterMenuSubPageElement subPageElement && subPageElement.Visible)
            {
                var teamUI = UIElement.CharacterMenuTeam.UiBaseAs<UI_CharacterMenuTeam>();
                var charTogGroup = teamUI.CGet<InfinityScroll>("CharScroll").GetComponent<CToggleGroup>();
                charTogGroup.Set(index);
            }
            
        }

    }
}