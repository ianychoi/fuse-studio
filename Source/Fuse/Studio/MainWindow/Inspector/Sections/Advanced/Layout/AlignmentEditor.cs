using Outracks.Fuse.Inspector.Editors;
using Outracks.Fusion;

namespace Outracks.Fuse.Inspector.Sections
{
	enum Alignment
	{
		Default,
		Left,
		HorizontalCenter,
		Right,
		Top,
		VerticalCenter,
		Bottom,
		TopLeft,
		TopCenter,
		TopRight,
		CenterLeft,
		Center,
		CenterRight,
		BottomLeft,
		BottomCenter,
		BottomRight,
	}

	class AlignmentEditor
	{
		public static IEditorControl Create(IAttribute<Alignment> attribute, IEditorFactory editors)
		{
			var smallPadding = Optional.Some(new Points(5.0));
			var largePadding = Optional.Some(new Points(16.0));

			return new EditorControl<Alignment>(
				editors,
				attribute,

				Layout.StackFromTop(
					Layout.StackFromLeft(
						CustomRadioButton.Create(
							attribute,
							Alignment.Default,
							"정렬: Default",
							(backgroundColor, color, stroke) =>
								CustomRadioButton.CreateBackgroundRect(backgroundColor, color))
						.WithPadding(right: smallPadding),

						CustomRadioButton.Create(
							attribute,
							Alignment.Center,
							"정렬: Center",
							(backgroundColor, color, stroke) => Layout.Layer(
								CustomRadioButton.CreateBackgroundRect(backgroundColor, color),
								CustomRadioButton.CreateSmallRect(color).Center()))
						.WithPadding(right: largePadding),

						CustomRadioButton.Create(
							attribute,
							Alignment.Bottom,
							"정렬: Bottom",
							(backgroundColor, color, stroke) => Layout.Layer(
								CustomRadioButton.CreateBackgroundRect(backgroundColor, color),
								Shapes.Rectangle(fill: color)
								.WithSize(new Size<Points>(CustomRadioButton.ButtonDim, CustomRadioButton.SmallRectSize.Height)).DockBottom()))
						.WithPadding(right: smallPadding),

						CustomRadioButton.Create(
							attribute,
							Alignment.Top,
							"정렬: Top",
							(backgroundColor, color, stroke) => Layout.Layer(
								CustomRadioButton.CreateBackgroundRect(backgroundColor, color),
								Shapes.Rectangle(fill: color)
								.WithSize(new Size<Points>(CustomRadioButton.ButtonDim, CustomRadioButton.SmallRectSize.Height)).DockTop()))
						.WithPadding(right: largePadding),

						CustomRadioButton.Create(
							attribute,
							Alignment.Left,
							"정렬: Left",
							(backgroundColor, color, stroke) => Layout.Layer(
								CustomRadioButton.CreateBackgroundRect(backgroundColor, color),
								Shapes.Rectangle(fill: color)
								.WithSize(new Size<Points>(CustomRadioButton.SmallRectSize.Height, CustomRadioButton.ButtonDim)).DockLeft()))
						.WithPadding(right: smallPadding),

						CustomRadioButton.Create(
							attribute,
							Alignment.Right,
							"정렬: Right",
							(backgroundColor, color, stroke) => Layout.Layer(
								CustomRadioButton.CreateBackgroundRect(backgroundColor, color),
								Shapes.Rectangle(fill: color)
								.WithSize(new Size<Points>(CustomRadioButton.SmallRectSize.Height, CustomRadioButton.ButtonDim)).DockRight()))
						.WithPadding(right: largePadding),

						CustomRadioButton.Create(
							attribute,
							Alignment.HorizontalCenter,
							"정렬: Horizontal center",
							(backgroundColor, color, stroke) => Layout.Layer(
								CustomRadioButton.CreateBackgroundRect(backgroundColor, color),
								Shapes.Line(
									new Point<Points>(0.0, 0.0),
									new Point<Points>(0.0, CustomRadioButton.ButtonDim - 4.0),
									stroke)
								.WithSize(new Size<Points>(1.0, CustomRadioButton.ButtonDim - 4.0)).Center(),
								CustomRadioButton.CreateSmallRect(color).Center()))
						.WithPadding(right: smallPadding),

						CustomRadioButton.Create(
							attribute,
							Alignment.VerticalCenter,
							"정렬: Vertical center",
							(backgroundColor, color, stroke) => Layout.Layer(
								CustomRadioButton.CreateBackgroundRect(backgroundColor, color),
								Shapes.Line(
									new Point<Points>(0.0, 0.0),
									new Point<Points>(CustomRadioButton.ButtonDim - 4.0, 0.0),
									stroke)
								.WithSize(new Size<Points>(CustomRadioButton.ButtonDim - 4.0, 1.0)).Center(),
								CustomRadioButton.CreateSmallRect(color).Center())))
					.WithPadding(bottom: new Points(9.0)),

					Layout.StackFromLeft(
						CustomRadioButton.Create(
							attribute,
							Alignment.BottomCenter,
							"정렬: Bottom center",
							(backgroundColor, color, stroke) => Layout.Layer(
								CustomRadioButton.CreateBackgroundRect(backgroundColor, color),
								CustomRadioButton.CreateSmallRect(color).DockBottom().CenterHorizontally()))
						.WithPadding(right: smallPadding),

						CustomRadioButton.Create(
							attribute,
							Alignment.TopCenter,
							"정렬: Top center",
							(backgroundColor, color, stroke) => Layout.Layer(
								CustomRadioButton.CreateBackgroundRect(backgroundColor, color),
								CustomRadioButton.CreateSmallRect(color).DockTop().CenterHorizontally()))
						.WithPadding(right: largePadding),

						CustomRadioButton.Create(
							attribute,
							Alignment.CenterLeft,
							"정렬: Center left",
							(backgroundColor, color, stroke) => Layout.Layer(
								CustomRadioButton.CreateBackgroundRect(backgroundColor, color),
								CustomRadioButton.CreateSmallRect(color).DockLeft().CenterVertically()))
						.WithPadding(right: smallPadding),

						CustomRadioButton.Create(
							attribute,
							Alignment.CenterRight,
							"정렬: Center right",
							(backgroundColor, color, stroke) => Layout.Layer(
								CustomRadioButton.CreateBackgroundRect(backgroundColor, color),
								CustomRadioButton.CreateSmallRect(color).DockRight().CenterVertically()))
						.WithPadding(right: largePadding),

						CustomRadioButton.Create(
							attribute,
							Alignment.BottomLeft,
							"정렬: Bottom left",
							(backgroundColor, color, stroke) => Layout.Layer(
								CustomRadioButton.CreateBackgroundRect(backgroundColor, color),
								CustomRadioButton.CreateSmallRect(color).DockBottomLeft()))
						.WithPadding(right: smallPadding),

						CustomRadioButton.Create(
							attribute,
							Alignment.BottomRight,
							"정렬: Bottom right",
							(backgroundColor, color, stroke) => Layout.Layer(
								CustomRadioButton.CreateBackgroundRect(backgroundColor, color),
								CustomRadioButton.CreateSmallRect(color).DockBottomRight()))
						.WithPadding(right: largePadding),

						CustomRadioButton.Create(
							attribute,
							Alignment.TopLeft,
							"정렬: Top left",
							(backgroundColor, color, stroke) => Layout.Layer(
								CustomRadioButton.CreateBackgroundRect(backgroundColor, color),
								CustomRadioButton.CreateSmallRect(color).DockTopLeft()))
						.WithPadding(right: smallPadding),

						CustomRadioButton.Create(
							attribute,
							Alignment.TopRight,
							"정렬: Top right",
							(backgroundColor, color, stroke) => Layout.Layer(
								CustomRadioButton.CreateBackgroundRect(backgroundColor, color),
								CustomRadioButton.CreateSmallRect(color).DockTopRight()))))
				.CenterHorizontally()
				.WithBackground(editors.ExpressionButton(attribute).DockRight()));
		}
	}
}
