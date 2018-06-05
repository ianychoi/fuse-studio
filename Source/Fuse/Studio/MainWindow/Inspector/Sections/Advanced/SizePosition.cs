﻿namespace Outracks.Fuse.Inspector.Sections
{
	using Fusion;
	
	class SizePositionSection
	{
		public static IControl Create(IElement element, IEditorFactory editors)
		{
			element = element.As("Fuse.Elements.Element");

			var x = element.GetSize("X", UxSize.Points(0.0));
			var y = element.GetSize("Y", UxSize.Points(0.0));

			//var offset = element.GetSize2("Offset", Size.Create(UxSize.Points(0.0), UxSize.Points(0.0))).Transpose(UxSize.Points(0.0));

			var width = element.GetPoints("Width", 0.0);
			var height = element.GetPoints("Height", 0.0);
			var maxWidth = element.GetPoints("MaxWidth", 0.0);
			var maxHeight = element.GetPoints("MaxHeight", 0.0);
			var minWidth = element.GetPoints("MinWidth", 0.0);
			var minHeight = element.GetPoints("MinHeight", 0.0);

			return Layout.StackFromTop(
				
				Spacer.Medium,
				Layout.Dock()
					.Left(Layout.StackFromTop(
							editors.Label(" ", width, height),
							Spacer.Smaller,
							editors.Label("너비", width),
							Spacer.Small,
							editors.Label("높이", height))
						.WithWidth(CellLayout.HalfCellWidth))
					.Left(Spacer.Small)
					.Left(Layout.StackFromTop(
							editors.Label(" ", width, height),
							Spacer.Smaller,
							editors.Field(width, toolTip: "Width (너비)"),
							Spacer.Small,
							editors.Field(height, toolTip: "Height (높이)"))
						.WithWidth(CellLayout.HalfCellWidth))
					.Right(Layout.StackFromTop(
							editors.Label("최대값", maxWidth, maxHeight),
							Spacer.Smaller,
							editors.Field(maxWidth, toolTip: "최대 너비값"),
							Spacer.Small,
							editors.Field(maxHeight, toolTip: "최대 높이값"))
						.WithWidth(CellLayout.HalfCellWidth))
					.Right(Spacer.Small)
					.Right(Layout.StackFromTop(
							editors.Label("최소값", minWidth, minHeight),
							Spacer.Smaller,
							editors.Field(minWidth, toolTip: "최소 너비값"),
							Spacer.Small,
							editors.Field(minHeight, toolTip: "최소 높이값"))
						.WithWidth(CellLayout.HalfCellWidth))
					.Fill(Spacer.Small)
					.WithInspectorPadding(),
				Spacer.Medium,
				Separator.Weak,
				Spacer.Medium,

				Layout.Dock()
					.Fill(
						Layout.Dock()
						.Left(editors.Label("X", x)
							.WithWidth(26)
							.DockLeft(editors.Field(x)).WithWidth(CellLayout.FullCellWidth))
						.Right(editors.Label("Y", y)
							.WithWidth(26)
							.DockLeft(editors.Field(y)).WithWidth(CellLayout.FullCellWidth))
						.Fill(Spacer.Medium))

					//.Right(Layout.StackFromTop(
					//		editors.Label("\u2194", offset.Width)
					//			.CenterHorizontally().WithWidth(26)
					//			.DockLeft(editors.Field(offset.Width)),
					//		Spacer.Small,
					//		editors.Label("\u2195", offset.Height)
					//			.CenterHorizontally().WithWidth(26)
					//			.DockLeft(editors.Field(offset.Height)))
					//	.WithWidth(CellLayout.FullCellWidth))

					
					.WithInspectorPadding(),

				Spacer.Medium, Separator.Weak);
		}
	}
}
