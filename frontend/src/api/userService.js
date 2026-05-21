import { useApiCall } from './apiClient';

export const useUserService = () => {
    const apiCall = useApiCall();

    return {
        getRecentGames: async (userId = 3, count = 4, includeAchievements = true) => {
            return apiCall(`/User/${userId}/recent-games/?count=${count}&includeAchievements=${includeAchievements}`,{method:'GET'});
        }
    };
};