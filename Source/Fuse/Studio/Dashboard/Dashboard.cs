using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Outracks.Fuse.Dashboard
{
	using Fusion;
	using IO;
	using Designer;

	class Dashboard
	{
		readonly ISubject<bool> _isVisible = new BehaviorSubject<bool>(false);

		public Dashboard(CreateProject createProject, IFuse fuse)
		{
			Application.Desktop.CreateSingletonWindow(
				isVisible: _isVisible,
				window: window => new Window
				{
					Style = WindowStyle.Fat,
					Title = Observable.Return("Fuse"),
					Size = Optional.Some(Property.Constant(Optional.Some(new Size<Points>(970, 608)))),
					Content = Control.Lazy(() => CreateContent(createProject, window, fuse.Version)),
					Foreground = Theme.DefaultText,
					Background = Theme.PanelBackground,
					Border = Separator.MediumStroke,
					HideTitle = true,
				});
			
		}

		public void Show()
		{
			_isVisible.OnNext(true);
		}

		IControl CreateContent(CreateProject createProject, IDialog<object> dialog,  string fuseVersion)
		{
			var projectList = new ProjectList(new Shell(), createProject, dialog);
			var openProjectFromDialog =
				Command.Enabled(() => Application.ShowOpenDocumentDialog(DocumentTypes.Project));

			return Popover.Host(popover =>
				Layout.Dock()
						.Top(
						CreateDashTopBar(fuseVersion)
						)
						.Top(Separator.Medium)

						.Right(Layout.StackFromTop(
							Layout.Dock()
							.Fill(
								Layout.SubdivideVertically(
										InfoItem("�н��ϱ�","Fuse�� ó���̽ʴϱ�? �ڵ��,\r\nƩ�丮�� �� ������ ���캾�ô�.","http://go.fusetools.com/tutorials", "Outracks.Fuse.Icons.Dashboard.Learn.png"),
										InfoItem("����","��� ������ �ʿ��Ͻʴϱ�?\r\n���۷��� �������� ã�ƺ�����.", "https://go.fusetools.com/docs", "Outracks.Fuse.Icons.Dashboard.Docs.png"),
										InfoItem("Ŀ�´�Ƽ","���� ���� ������ �ͽ��ϱ�? Fuse�� \r\n������ �ִ� Ŀ�´�Ƽ �������� �Բ��ϼ���.", "https://go.fusetools.com/community","Outracks.Fuse.Icons.Dashboard.Community.png" ))
								.WithPadding(
										left: new Points(32),
										top: new Points(40))
								.WithHeight(300)),
								Separator.Weak,
								CreateOpenButtons(openProjectFromDialog, projectList)
								)
							.WithWidth(260)
							.WithHeight(600)
							.WithBackground(Theme.PanelBackground))
						.Fill(projectList.Page)
						.WithBackground(Theme.WorkspaceBackground)
						.WithHeight(608)
						.WithWidth(970));

		}


		private static IControl InfoItem(string title, string message, string linkUrl, string icon)
		{
			return

				Layout.StackFromTop(
					Layout.StackFromLeft(
							Image.FromResource(icon, typeof(Dashboard).Assembly)
							.WithPadding(
									right: new Points(8)),
							Buttons.TextButton(
								text: title,
								color: Theme.DefaultText,
								font: Theme.HeaderFont,
								hoverColor: Theme.ActiveHover,
								cmd: Command.Enabled(action: () => Process.Start(linkUrl)))),
					Label.Create(
							message,
							color: Theme.DescriptorText,
							font: Theme.DescriptorFont)
					.WithPadding(top: new Points(8)));
		}

		private static IControl CreateOpenButtons(Command openProjectFromDialog, ProjectList projectList)
		{
			return
			Layout.StackFromLeft(
					Buttons.DefaultButton(
							text: "Ž��", 
							cmd: openProjectFromDialog)
						.WithWidth(104),
					Buttons.DefaultButtonPrimary(
							text: projectList.DefaultMenuItemName.AsText(),
							cmd: projectList.DefaultCommand)
						.WithWidth(104)
						.WithPadding(
							left: new Points(16)))
				.WithPadding(
					top: new Points(16),
					right: new Points(16),
					left: new Points(16));
		}

		private static IControl CreateDashTopBar(string fuseVersion)
		{

			var versionStr = "V" + fuseVersion;

			return
				Layout.Dock()

					.Right(
						Layout.Dock().Right(
							Layout.StackFromRight(
								Control.Empty.WithWidth(16).ShowOnMac(),
								Control.Empty.WithWidth(8).ShowOnWindows()))
								.Fill(
						Layout.StackFromRight(
								Label.Create(
										versionStr,
										color: Theme.DescriptorText,
										font: Theme.DefaultFont)
									.SetToolTip("Version " + fuseVersion),
								Label.Create(
										"Fuse Studio",
										color: Theme.DefaultText,
										font: Theme.DefaultFont)
									.WithPadding(
										right: new Points(4))))
							.CenterVertically())

					.Left(
						Layout.StackFromLeft(
								Control.Empty
								.WithWidth(80)
								.ShowOnMac(),
								Control.Empty
								.WithWidth(16)
								.ShowOnWindows(),
								Layout.Dock()
									.Bottom(
										Shapes.Rectangle(
												fill: Theme.Active)
										.WithHeight(1))
									.Fill(
										Buttons.TextButton(
												text: "�ֱ� ������Ʈ",
												color: Theme.Active,
												font: Theme.DefaultFont,
												hoverColor: Theme.ActiveHover,
												cmd: Command.Enabled())
										.CenterVertically()))
								.WithHeight(37))
					.Fill()
					.WithBackground(Theme.PanelBackground)
					.WithHeight(37);
		}

	}


	static class Selectable
	{
		public static IControl Create<T>(
			ISubject<T> selection,
			IObservable<T> data,
			Func<SelectionState, IControl> control,
			Menu menu,
			Command defaultCommand,
			Text toolTip = default(Text))
		{
			return control(
				new SelectionState
				{
					IsSelected = selection.CombineLatest(data, (s, d) => s.Equals(d)).Replay(1).RefCount(),
				})
				.WithBackground(Color.AlmostTransparent)
					.OnMouse(
						pressed: data.Switch(selection.Update),
						doubleClicked: defaultCommand)
					.SetContextMenu(menu)
					.SetToolTip(toolTip);
		}
	}

	public class SelectionState
	{
		public IObservable<bool> IsSelected { get; set; }
	}
}
