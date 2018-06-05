using System.Reactive.Linq;
using Outracks.Fuse.Setup;
using Outracks.IO;

namespace Outracks.Fuse.Designer
{
	using Diagnostics;
	using Fusion;
	using Stage;

	static class MainMenu 
	{
		public static Menu Create(
			IFuse fuse,
			IShell shell,
			StageController stage,
			Help help,
			Menu elementMenu,
			Menu projectMenu,
			Build preview,
			Export export,
			SetupGuide setupGuide,
			Menu windowMenu,
			Debug debug)
		{
			var about = new About(fuse.Version, debug);

			var toolsMenu 
				= setupGuide.Menu 
				+ Menu.Separator;

			var menus 
				= Menu.Submenu("����", CreateFileMenu(fuse))
				+ Menu.Submenu("����", Application.EditMenu)
				+ Menu.Submenu("�׸�", elementMenu)
				+ Menu.Submenu("������Ʈ", projectMenu)
				+ Menu.Submenu("����Ʈ", stage.Menu)
				+ Menu.Submenu("�̸�����", preview.Menu)
				+ Menu.Submenu("��������", export.Menu)
				+ Menu.Submenu("����", toolsMenu)
				+ Menu.Submenu("â", windowMenu)
				+ Menu.Submenu("����", CreateHelpMenu(fuse, help, about.Menu))
				+ debug.Menu;

			var fuseMenu = Menu.Submenu("Fuse", CreateFuseMenu(fuse, about.Menu));

			if (fuse.Platform == OS.Mac)
				return fuseMenu + menus;
			
			return menus;
		}

		private static Menu CreateHelpMenu(IFuse fuse, Help help, Menu aboutMenuItem)
		{
			return fuse.Platform == OS.Windows ? help.Menu + Menu.Separator + aboutMenuItem : help.Menu;
		}

		static Menu CreateFuseMenu(IFuse fuse, Menu aboutMenuItem)
		{
			return aboutMenuItem
				 + Menu.Separator
				 + CreateQuitItem(fuse);
		}

		static Menu CreateFileMenu(IFuse fuse)
		{
			return CreateSplashScreenItem()
				+ CreateOpenItem()
				+ (fuse.Platform == OS.Windows
					? Menu.Separator + CreateQuitItem(fuse)
					: Menu.Empty);
		}


		static Menu CreateSplashScreenItem()
		{
			return Menu.Item(
				"�� ������Ʈ...",
				hotkey: HotKey.Create(ModifierKeys.Meta, Key.N),
				action: () => Application.LaunchedWithoutDocuments());
		}

		static Menu CreateOpenItem()
		{
			return Menu.Item(
				"����...",
				hotkey: HotKey.Create(ModifierKeys.Meta, Key.O),
				action: () => Application.ShowOpenDocumentDialog(new FileFilter("Fuse Project", "unoproj")));
		}


		static Menu CreateQuitItem(IFuse fuse)
		{
			var quitOrExit = fuse.Platform == OS.Mac ? "Quit" : "Exit";
			var quitItem = Menu.Item(
				quitOrExit + " " + "Fuse",
				hotkey: fuse.Platform == OS.Windows ? HotKey.Create(ModifierKeys.Alt, Key.F4) : HotKey.Create(ModifierKeys.Command, Key.Q),
				action: () => { Application.Exit(0); });
			return quitItem;
		}


	}
}
