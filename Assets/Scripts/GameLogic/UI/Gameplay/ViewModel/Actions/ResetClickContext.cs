namespace GameLogic.UI.Gameplay
{
    public class ResetClickContext : BaseGameplayViewModelAction
    {
        public override void Execute(GameplayViewModelContext context)
        {
            foreach (var pair in context.Click.WordRowHintClusters)
            {
                var hintCluster = pair.Value;
                if (hintCluster == null)
                    continue;

                context.AllClusters.Remove(hintCluster);
                hintCluster.Dispose();
            }
            context.Click.WordRowHintClusters.Clear();

            if (context.Click.HintUndistributedClickedCluster != null)
                context.Click.HintUndistributedClickedCluster.Dispose();

            context.Click.OriginUndistributedClickedCluster = null;
            context.Click.HintUndistributedClickedCluster = null;
            context.Click.ClickedHintWordRow = null;

            context.Click.IsClickInputNow = false;
        }
    }
}