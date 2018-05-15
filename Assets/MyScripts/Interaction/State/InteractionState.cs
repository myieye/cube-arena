namespace CubeArena.Assets.MyScripts.Interaction.State {
    public enum InteractionState { Idle, Moving, Rotating, Selected, Disallowed, Spray }

	public static class InteractionStateHelpers {
		public static bool IsSelectionState (this InteractionState state) {
			switch (state) {
				case InteractionState.Moving:
				case InteractionState.Rotating:
				case InteractionState.Selected:
				case InteractionState.Disallowed:
					return true;
				case InteractionState.Idle:
				case InteractionState.Spray:
				default:
					return false;
			}
		}
	}
}