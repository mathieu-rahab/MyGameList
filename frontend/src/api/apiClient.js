import { useCookies } from 'react-cookie';

const API_BASE = 'http://localhost:5131/api/';

// Hook pour récupérer le token
export const useApiCall = () => {
    const [cookies] = useCookies(['jwt_token']);

    return async (endpoint, options = {}) => {
        const token = cookies.jwt_token;
        const { responseType = 'json', ...fetchOptions } = options;

        const headers = {
            'Content-Type': 'application/json',
            ...fetchOptions.headers,
        };

        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }

        try {
            const response = await fetch(endpoint.startsWith('http') ? endpoint : `${API_BASE}${endpoint}`, {
                ...fetchOptions,
                headers,
            });

            if (!response.ok) {
                let err = null;
                try {
                    err = await response.json();
                } catch {}
                return Promise.reject({
                    status: response.status,
                    error: err?.error
                });
            }

            // Retourner selon le type de réponse attendu
            return responseType === 'text' ? await response.text() : await response.json();
        } catch (error) {
            throw error;
        }
    };
};