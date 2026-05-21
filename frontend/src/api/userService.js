import { useApiCall } from './apiClient';

export const useUserService = () => {
    const apiCall = useApiCall();

    return {
        getRecentGames: async (userId, count = 4, includeAchievements = false, l = "french") => {
            return apiCall(`/User/${userId}/recent-games/?count=${count}&includeProgression=${includeAchievements}&l=${l}`,{method:'GET'});
        },

        getRecentAchievements: async (userId, count = 4, withPercent, l = "french") => {
            return apiCall(`/User/${userId}/recent-achievements/?count=${count}&includeRarity=${withPercent}&l=${l}`,{method:'GET'});

        }
    };
};