using System;
using System.Drawing.Drawing2D;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Outracks.Fuse.Designer.Icons;
using Outracks.Fuse.Stage;

namespace Outracks.Fuse.Designer
{
	using Fusion;
	using Outracks.Fuse.Inspector.Editors;
	
	public interface IInspectorControl
	{
		IControl Create();
	}

	static class MainWindow
	{
		public static Points ResizeBorderSize = 2;
	
		public static Window Create(
			IObservable<string> projectName,
			IControl search,
			IControl outline,
			IControl bookmarks,
			StageController stage,
			IControl notifications,
			IControl inspector,
			IControl attributes,
			LogView logview,
			Menu menu,
			Command closed,
			IProperty<bool> selectionEnabled,
			IObservable<bool> topMost,
			IProperty<Mode> mode,
			Command mainWindowFocused,
			IContext context,
			CodeView codeView)
		{
			IControl topBar = null;
			
			var newView = stage.NewViewport;
			
			var content = 
				Popover.Host(
					popover =>
					{
						topBar = CreateTopBar(
							popover,
							selectionEnabled: selectionEnabled,
							mode: mode,
							addViewport: newView,
							codeView: codeView);

						return CreateContent(
							logview: logview,
							outline: outline,
							search: search,
							notifications: notifications,
							attributes: attributes,
							bookmarks: bookmarks,
							inspector: inspector,
							stage: new StageView(stage, context, mode),
							topBar: topBar,
							mode: mode);
					});
			
			var mainWindowSize = UserSettings.Size("MainWindowSize");
			var mainWindowPosition = UserSettings.Position("MainWindowPosition");
			var mainWindowState = UserSettings.Settings.Property<WindowState>("MainWindowState");

			var compactSize = UserSettings.Size("CompactWindowSize");
			var compactPosition = UserSettings.Position("CompactWindowPosition");


			var compactContentSize =
				compactSize.With(value:
					compactSize.Merge(
						stage.DefaultViewportSize.Select(Optional.Some)
							.CombineLatest(
								topBar.DesiredSize.Height,
								(deviceSize, topBarHeight) =>
									deviceSize.Select(s => 
										Size.Create(
											s.Width + ResizeBorderSize * 2 /* padding */,
											s.Height + topBarHeight + ResizeBorderSize * 2 + LogViewHeader.HeaderHeight /* padding */)))));
			

			var windowSize = mode.Select(x => x == Mode.Normal ? mainWindowSize : compactContentSize).Switch();
			var windowState = mode.Select(x => x == Mode.Normal ? mainWindowState : Property.Constant(Optional.Some(WindowState.Normal))).Switch();
			var windowPosition = mode.Select(x => x == Mode.Normal ? mainWindowPosition : compactPosition).Switch();


			return new Window
			{	
				Closed = closed,
				Title = projectName
					.Select(Optional.Some)
					.StartWith(Optional.None())
					.Select(
						maybePath => maybePath.MatchWith(
							some: name => name + " - Fuse",
							none: () => "Fuse")),
				Size = Optional.Some(windowSize),
				Position = Optional.Some(windowPosition),
				State = Optional.Some(windowState),
				Menu = menu,
				Content = content,
				TopMost = Optional.Some(topMost),
				Focused = mainWindowFocused,
				Foreground = Theme.DefaultText,
				Background = Theme.PanelBackground,
				Border = Separator.MediumStroke,
				Style = WindowStyle.Fat
			};
		}
		
		public static IControl CreateContent(
			LogView logview,
			IControl outline,
			IControl search,
			IControl notifications,
			IControl attributes,
			IControl bookmarks,
			IControl inspector,
			StageView stage,
			IControl topBar,
			IObservable<Mode> mode)
		{
			Points inspectorWidth = 295;

			var right = Property.Constant(Fuse.Inspector.Inspector.Width);
			var left = UserSettings.Point("LeftPanelWidth").Or(new Points(260));
			
			var inNormalMode = mode.Select(x => x == Mode.Normal);

			return Layout.Layer(
				Shapes.Rectangle(fill: Theme.WorkspaceBackground),
				Layout.Dock()
					.Top(topBar)
					.Top(Separator.Medium)

					.Panel(RectangleEdge.Right, right,
						inNormalMode,
						control: inspector,
						minSize: inspectorWidth,
						resizable: false)

					.Panel(RectangleEdge.Left, left,
						inNormalMode,
						control: outline,
						minSize: 10)

					.Fill(stage.CreateStage(logview, notifications)));
						
		}

		public static DockBuilder Panel(
			this DockBuilder dock, RectangleEdge dockEdge, IProperty<Points> size,
			IObservable<bool> isExpanded,
			IControl control, 
			Points minSize, 
			bool resizable = true)
		{
			var availableSize = new BehaviorSubject<Size<IObservable<Points>>>(
				new Size<IObservable<Points>>(Observable.Return<Points>(double.MaxValue), Observable.Return<Points>(double.MaxValue)));
			var maxWidth = availableSize.Switch()[dockEdge.NormalAxis()];

			control = control
				.WithBackground(Theme.PanelBackground)
				.WithFrame(f => f, a => a.WithAxis(dockEdge.NormalAxis(), s => size.Min(maxWidth)))
				.WithDimension(dockEdge.NormalAxis(), size.Min(maxWidth));

			control = Layout.Dock()
				.Dock(edge: dockEdge, control: control)
				.Dock(edge: dockEdge, control: Separator.Medium)
				.Fill();

			if (resizable)
				control = control.MakeResizable(dockEdge.Opposite(), size, minSize: minSize);

			control = control.MakeCollapsable(dockEdge.Opposite(), isExpanded, lazy: false);
			
			control = control.WithFrame(
				frame => frame,
				availSize =>
				{
					availableSize.OnNext(availSize);
					return availSize;
				});

			return dock.Dock(edge: dockEdge, control: control);
		}
		
		
		static IControl CreateHeaderControl(
			Command command,
			string buttonText,
			string tooltipText,
			IControl icon,
			Brush labelColor
			)
		{
			return
				Button.Create(command, state => 
						Layout.StackFromLeft(
								Control.Empty.WithWidth(4),
								icon,
								Control.Empty.WithWidth(4),
								Label.Create(
										text: buttonText,
										color:labelColor,
										font: Theme.DescriptorFont)
									.CenterVertically(),
								Control.Empty.WithWidth(4))
							.SetToolTip(tooltipText)
							.WithBackground(
								background: Observable.CombineLatest(
										state.IsEnabled, state.IsHovered,
										(enabled, hovering) =>
											hovering
												? Theme.FaintBackground
												: Color.Transparent)
									.Switch()));
		}
		
		
		
		static IControl CreateFullSelectionControl(
			IProperty<bool> selectionEnabled)
		{

			return
				Layout.StackFromLeft(
					Layout.Dock()
						.Bottom(
							Shapes.Rectangle(
									fill: selectionEnabled.IsFalse()
										.Select(e => e ? Color.Transparent : Theme.Active)
										.Switch())
								.WithHeight(1))
						.Fill(
							CreateHeaderControl(
								icon: SelectionIcon.Create(selectionEnabled, true),
								tooltipText: "앱에서 항목 선택을 활성화합니다.",
								buttonText : "선택",
								labelColor: selectionEnabled.IsFalse()
									.Select(e => e ? Theme.DefaultText : Theme.ActiveHover)
									.Switch(),
								command: selectionEnabled.Toggle())),
					Control.Empty.WithWidth(24),
					Layout.Dock()
						.Bottom(
							Shapes.Rectangle(
									fill: selectionEnabled.IsFalse()
										.Select(e => e ? Theme.Active : Color.Transparent)
										.Switch())
								.WithHeight(1))
						.Fill(
							CreateHeaderControl(
								icon: TouchIcon.Create(selectionEnabled, true),
								tooltipText: "앱에서 인터렉션을 활성화합니다.",
								buttonText : "터치",
								labelColor: selectionEnabled
									.Select(e => e ? Theme.DefaultText : Theme.ActiveHover)
									.Switch(),
								command: selectionEnabled.Toggle())));
		}
		
		static IControl CreateCompactSelectionControl(
			IProperty<Mode> mode,
			IProperty<bool> selectionEnabled,
			Command toggleMode)
		{

			return
				Layout.StackFromLeft(
					Button.Create(selectionEnabled.Toggle(), state =>
						Layout.Dock()
							.Bottom(
								Shapes.Rectangle(
										fill: Theme.Active)
									.WithSize(new Size<Points>(1, 1)))
							.Fill(
								Layout.StackFromLeft(
									SelectionIcon.Create(selectionEnabled, true)
										.OnMouse(pressed: selectionEnabled.Toggle())
										.ShowWhen(selectionEnabled),
									TouchIcon.Create(selectionEnabled, true)
										.Center()
										.OnMouse(pressed: selectionEnabled.Toggle())
										.ShowWhen(selectionEnabled.IsFalse())))
							.WithPadding(new Thickness<Points>(4, 0, 4, 0))
							.WithBackground(
								background: Observable.CombineLatest(
										state.IsEnabled, state.IsHovered,
										(enabled, hovering) =>
											hovering
												? Theme.FaintBackground
												: Color.Transparent)
									.Switch())
							.SetToolTip("앱에서 항목을 선택하여 활성화합니다. 비활성화하면 앱에서 인터렉션을 허용합니다.")),
					Control.Empty.WithWidth(8),
					Button.Create(toggleMode, state =>
							MinimizeAndMaximizeIcon.Create(mode)
							.WithPadding(new Thickness<Points>(4, 0, 4, 0))
							.WithBackground(
								background: Observable.CombineLatest(
										state.IsEnabled, state.IsHovered,
										(enabled, hovering) =>
											hovering
												? Theme.FaintBackground
												: Color.Transparent)
									.Switch()))
							.SetToolTip("일반 모드 및 컴팩트 모드를 전환합니다. 클릭하여 전환하세요."));
		}

		static IControl CreateTopBar(
			IPopover popover,
			IProperty<bool> selectionEnabled,
			IProperty<Mode> mode,
			Command addViewport,
			CodeView codeView)
		{
			var isCompact = mode.Convert(m => m == Mode.Compact, m => m ? Mode.Compact : Mode.Normal);
			var toggleMode = isCompact.Toggle();
			
			return
				Layout.Layer(
					Layout
						.StackFromLeft(	
							codeView.Create(popover)
								.HideWhen(isCompact),
							
							Control.Empty.WithWidth(16),
							
							Layout
								.StackFromLeft(CreateHeaderControl(
									icon: Fuse.Icons.AddViewport(),
									tooltipText: "클릭하여 새로운 뷰포트를 추가합니다",
									buttonText : "뷰포트 추가",
									command: addViewport,
									labelColor: Theme.DefaultText),
									Control.Empty.WithWidth(16)
								)
								.HideWhen(isCompact),
								
							CreateHeaderControl(
									icon: MinimizeAndMaximizeIcon.Create(mode),
									tooltipText: "일반 모드 및 컴팩트 모드를 전환합니다. 클릭하여 전환하세요.",
									buttonText : "컴팩트",
									labelColor: Theme.DefaultText,
									command: toggleMode)
								.HideWhen(isCompact),
	
							CreateCompactSelectionControl(mode, selectionEnabled,toggleMode)
								.ShowWhen(isCompact)
								.Center(),
								
							Control.Empty.WithWidth(4))
						.DockRight(),
						CreateFullSelectionControl(selectionEnabled)
							.HideWhen(isCompact)
							.CenterHorizontally())
				.WithHeight(37)
				.WithPadding(new Thickness<Points>(8, 0, 8, 0))
				.WithBackground(Theme.PanelBackground);
		}

	}
}
