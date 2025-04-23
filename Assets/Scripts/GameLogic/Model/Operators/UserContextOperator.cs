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
    }
}