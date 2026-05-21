import { useApiCall } from './apiClient';

export const useUserService = () => {
    const apiCall = useApiCall();

    return {
        getRecentGames: async (userId, count = 4, includeAchievements = true) => {
            return apiCall(`/User/${userId}/recent-games/?count=${count}&includeAchievements=${includeAchievements}`,{method:'GET'});
        }
    };
};