using System.Collections.Generic;
using GameLogic.Model.Repositories;

namespace GameLogic.Model.Operators
{
    public class UserContextOperator
    {
        private readonly UserContextRepository _userContextRepository;

        public UserContextOperator(UserContextRepository userContextRepository)
        {
            _userContextRepository = userContextRepository;
        }

        public void ClearProgress()
        {
            _userContextRepository.ClearProgress();
            _userContextRepository.Save();
        }

        public void SetSoundsMuted(bool isMuted)
        {
            _userContextRepository.SetSoundsMuted(isMuted);
            _userContextRepository.Save();
        }
        
        public void SetMusicMuted(bool isMuted)
        {
            _userContextRepository.SetMusicMuted(isMuted);
            _userContextRepository.Save();
        }

        public void UpdateLocalization(string localizationDefId)
        {
            _userContextRepository.SetLocalization(localizationDefId);
            _userContextRepository.Save();
        }

        public void CompleteLevel(string levelDefId)
        {
            _userContextRepository.CompleteLevel(levelDefId);
            _userContextRepository.Save();
        }

        public void AddOrUpdateLevelProgress(string needLevelDefId, List<string> undistributedClusters, List<List<string>> distributedClusters)
        {
            _userContextRepository.AddOrUpdateLevelProgress(needLevelDefId, undistributedClusters, distributedClusters);
            _userContextRepository.Save();
        }

        public void SetHowToPlayHintShown()
        {
            _userContextRepository.SetHowToPlayHintShown();
            _userContextRepository.Save();
        }

        public bool TryUpdateFreeConsumablesCount()
        {
            if (_userContextRepository.TryUpdateFreeConsumablesCount())
            {
                _userContextRepository.Save();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void IncrementSaves(string levelDefId)
        {
            _userContextRepository.IncrementSaves(levelDefId);
            _userContextRepository.Save();
        }

        public void UseCheckingWords()
        {
            _userContextRepository.UseCheckingWords();
            _userContextRepository.Save();
        }

        public void AddCheckingWords()
        {
            _userContextRepository.AddCheckingWords();
            _userContextRepository.Save();
        }

        public void AddAdsTip()
        {
            _userContextRepository.AddAdsTip();
            _userContextRepository.Save();
        }

        public void UseAdsTip()
        {
            _userContextRepository.UseAdsTip();
            _userContextRepository.Save();
        }
    }
}