import { Navigate } from 'react-router-dom';
import { useAuth } from '../utils/useAuth';
import { useLocation } from 'react-router-dom';

export default function ProtectedRoute({ children }) {
    const { user, loading, isAuthenticated } = useAuth();
    const location = useLocation();

    // Pendant le chargement de l'auth, on ne montre rien
    if (loading) {
        return <div>{}</div>;
    }

    // Si pas authentifié, rediriger vers login avec le paramètre de redirection
    if (!isAuthenticated || !user) {
        const currentPage = location.pathname + location.search;
        return <Navigate
            to={`/login/?redirect=${encodeURIComponent(currentPage)}`}
            replace
        />;
    }

    return children;
}