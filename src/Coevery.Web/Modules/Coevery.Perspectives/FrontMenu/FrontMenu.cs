﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Coevery.Common.Extensions;
using Coevery.Common.Models;
using Coevery.Core.Navigation.Models;
using Coevery.Core.Navigation.Services;
using Coevery.Core.Navigation.ViewModels;
using Coevery.Core.Title.Models;
using Coevery.Localization;
using Coevery.Perspectives.Models;
using Coevery.UI;
using Coevery.UI.Navigation;
using Coevery.ContentManagement;
using System.Linq;

namespace Coevery.Perspectives.FrontMenu {
    public class FrontMenu : INavigationProvider {
        private readonly IMenuService _menuService;
        private readonly IContentDefinitionExtension _contentDefinitionExtension;

        public FrontMenu(
            ICoeveryServices coeveryServices,
            IContentDefinitionExtension contentDefinitionExtension,
            IMenuService menuService) {
            _contentDefinitionExtension = contentDefinitionExtension;
            Services = coeveryServices;
            _menuService = menuService;
        }

        public Localizer T { get; set; }

        public string MenuName {
            get { return "FrontMenu"; }
        }

        public ICoeveryServices Services { get; set; }

        public void GetNavigation(NavigationBuilder builder) {
            var menus = Services.ContentManager.Query<PerspectivePart, PerspectivePartRecord>().OrderBy(x => x.Position).ForType("Menu").List().ToList();
            menus.ForEach(c => {
                builder.AddImageSet(c.Title)
                    .Add(T(c.Title), c.Position.ToString(),
                        menu => {
                            var url = string.Format("#/{0}", c.Id);
                            menu.Url(url);
                            menu.Content(c);
                            menu.IdHint(c.Id.ToString(CultureInfo.InvariantCulture));
                            var subMenus = _menuService.GetMenuParts(c.Id)
                                .OrderBy(menuPartEntry => menuPartEntry.MenuPosition, new FlatPositionComparer())
                                .Where(menuPartEntry =>
                                    menuPartEntry.MenuPosition.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Length == 1)
                                .ToList();
                            foreach (var subMenu in subMenus) {
                                AddChildMenuItem(subMenu, url, ref menu);
                            }
                        });
            });
        }

        private static int GetLevel(string position) {
            var orderList = position.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            return orderList.Length;
        }

        private IEnumerable<MenuPart> GetChildMenuParts(string position) {
            var rawList = Services.ContentManager.Query<MenuPart, MenuPartRecord>()
                .Where(menu => menu.MenuPosition.StartsWith(position + "."))
                .ForPart<MenuPart>().List();
            return rawList == null || !rawList.Any() ? null : rawList.Where(menu => GetLevel(menu.MenuPosition) == GetLevel(position) + 1)
                .OrderBy(menuPartEntry => menuPartEntry.MenuPosition, new FlatPositionComparer());
        }

        private void AddChildMenuItem(MenuPart childMenu, string baseUrl, ref NavigationItemBuilder builder) {
            var subMenuCotent = childMenu;
            var menuItemEntity = CreateMenuItemEntries(childMenu, baseUrl);
            Action<NavigationItemBuilder> addChildrenAction = item => {
                item.Url(menuItemEntity.Url)
                    .Content(subMenuCotent)
                    .IdHint(subMenuCotent.Id.ToString(CultureInfo.InvariantCulture));
                var grandChildMenus = GetChildMenuParts(subMenuCotent.MenuPosition);
                if (grandChildMenus == null) {
                    return;
                }
                foreach (var grandChildMenu in grandChildMenus) {
                    AddChildMenuItem(grandChildMenu, baseUrl, ref item);
                }
            };
            if (!childMenu.Is<ModuleMenuItemPart>()) {
                builder.Add(T(menuItemEntity.Text), menuItemEntity.Position, addChildrenAction);
            } else {
                var moduleMenuItem = childMenu.As<ModuleMenuItemPart>();
                builder.Add(T(menuItemEntity.Text), menuItemEntity.Position, addChildrenAction,
                    new List<string> { moduleMenuItem.IconClass });
            }
        }

        private MenuItemEntry CreateMenuItemEntries(MenuPart menuPart, string parentUrl) {
            string urlFormat = parentUrl + "/{0}";
            string url;

            if (menuPart.Is<ModuleMenuItemPart>()) {
                var urlName = _contentDefinitionExtension.GetEntityNames(
                    menuPart.As<ModuleMenuItemPart>().ContentTypeDefinitionRecord.Name).CollectionName;
                url = string.Format(urlFormat, urlName);
            }
                //@todo: add process for other category of navigation item
            else {
                url = string.Format(urlFormat, menuPart.MenuText);
            }

            return new MenuItemEntry {
                MenuItemId = menuPart.Id,
                IsMenuItem = menuPart.Is<MenuItemPart>(),
                Text = menuPart.MenuText,
                Position = menuPart.MenuPosition,
                Url = url,
                ContentItem = menuPart.ContentItem,
            };
        }
    }
}