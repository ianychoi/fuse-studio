namespace Outracks.Fuse.Inspector.Sections
{
	using Fusion;

	static class RotationSection
	{
		public static IControl Create(IElement element, IEditorFactory editors)
		{
			return editors.ElementList(
				"회전", element,
				SourceFragment.FromString("<Rotation/>"),
				rotation => CreateRotationRow(rotation, editors)
					.WithInspectorPadding());
		}

		static IControl CreateRotationRow(IElement rotation, IEditorFactory editors)
		{
			var name = rotation.UxName();
			var degrees = rotation.GetDouble("Degrees", 0.0);

			return Layout.StackFromTop(
				editors.NameRow("Rotation Name", name),

				Spacer.Medium,

				Layout.Dock()
					.Left(editors.Label("각도", degrees))
					.Left(Spacer.Medium)
					.Right(editors.Field(degrees).WithWidth(CellLayout.HalfCellWidth))
					.Right(Spacer.Medium)
					.Fill(editors.Slider(degrees, min: -180.0, max: 180.0)));
		}
	}
}
