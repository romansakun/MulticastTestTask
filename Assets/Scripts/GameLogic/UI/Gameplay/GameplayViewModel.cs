using GameLogic.Model.DataProviders;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class GameplayViewModel : ViewModel 
    {
        [Inject] private Cluster.Pool _clusterPool;
        [Inject] private GuessWord.Pool _guessWordPool;
        [Inject] private UserContextDataProvider _userContext;

        private RectTransform _distributedClustersHolder;
        private RectTransform _guessWordsHolder;
        

        public override void Initialize()
        {
            var cluster = _clusterPool.Spawn(_clusterPool);
            cluster.SetText("Hello World");
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void SetHolders(RectTransform undistributedClustersHolder, RectTransform wordsHolder)
        {
            _distributedClustersHolder = undistributedClustersHolder;
            _guessWordsHolder = wordsHolder;
        }
        
        
        public void OnPointerClick(PointerEventData eventData)
        {
            // _logicAgent.Context.InputType = GameplayViewModelContext.PointerInputType.OnClick;
            // _logicAgent.Context.PointerEventData = eventData;
            // _logicAgent.Execute();
            
            Debug.Log("Click");
        }

        public virtual void OnPointerBeginDrag(PointerEventData eventData)
        {
            // _logicAgent.Context.InputType = GameplayViewModelContext.PointerInputType.OnDragBegin;
            // _logicAgent.Context.PointerEventData = eventData;
            // _logicAgent.Execute();
            
            Debug.Log("Begin Drag");
        }

        public virtual void OnPointerDrag(PointerEventData eventData)
        {
            // _logicAgent.Context.InputType = GameplayViewModelContext.PointerInputType.OnDrag;
            // _logicAgent.Context.PointerEventData = eventData;
            // _logicAgent.Execute();
            
            Debug.Log("Drag");
        }

        public virtual void OnPointerEndDrag(PointerEventData eventData)
        {
            // _logicAgent.Context.InputType = GameplayViewModelContext.PointerInputType.OnDragEnd;
            // _logicAgent.Context.PointerEventData = eventData;
            // _logicAgent.Execute();
            
            Debug.Log("End Drag");
        }
        //
        // public bool TryGetCellByPointerPosition(out Cell cell)
        // {
        //     for (int i = Cells.Count - 1; i >= 0; i--)
        //     {
        //         cell = Cells[i];
        //         if (RectTransformUtility.RectangleContainsScreenPoint(cell.RectTransform, PointerEventData.position, null, Vector4.zero)) 
        //             return true;
        //     }
        //     cell = null;
        //     return false;
        // }

        public void OnCheckWordsButtonClicked()
        {
            var guessWord = _guessWordPool.Spawn(_guessWordPool);
            guessWord.SetParent(_guessWordsHolder);
            guessWord.AddDummyToSuitablePosition(Vector2.zero);
            guessWord.AddDummyToSuitablePosition(Vector2.zero);
            guessWord.AddDummyToSuitablePosition(Vector2.zero);
        }
    }
}