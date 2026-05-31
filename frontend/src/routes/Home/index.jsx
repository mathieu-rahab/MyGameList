import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useUserService } from "../../api/userService.js";
import "./index.css";

export default function Home() {
    const { t, i18n } = useTranslation();
    const { searchGames } = useUserService();

    const [searchTerm, setSearchTerm] = useState("");
    const [searchResults, setSearchResults] = useState([]);
    const [loading, setLoading] = useState(false);
    const [imageErrors, setImageErrors] = useState({});

    const handleImageError = (gameId) => {
        setImageErrors(prev => ({ ...prev, [gameId]: true }));
    };

    useEffect(() => {
        const timeoutId = setTimeout(async () => {
            if (!searchTerm.trim()) {
                setSearchResults([]);
                setLoading(false);
                return;
            }

            setLoading(true);
            try {
                const results = await searchGames(searchTerm, i18n.language);
                setSearchResults(results || []);
            } catch (err) {
                console.error("Search error:", err);
                setSearchResults([]);
            } finally {
                setLoading(false);
            }
        }, 400);

        return () => clearTimeout(timeoutId);
    }, [searchTerm, i18n.language]); // eslint-disable-line react-hooks/exhaustive-deps

    return (
        <main className="search-page">
            <div className="search-header">
                <h1>{t('Search.SearchGames')}</h1>
                <div className="search-input-wrapper">
                    <i className="ti ti-search" aria-hidden="true"></i>
                    <input
                        type="text"
                        placeholder={t('Collection.SearchPlaceholder')}
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        autoFocus
                    />
                </div>
            </div>

            <div className="search-content">
                {searchTerm.length === 0 ? (
                    <div className="empty-state glass">
                        <i className="ti ti-device-gamepad-2"></i>
                        <p>{t('Search.EnterSearchTerm')}</p>
                    </div>
                ) : loading ? (
                    <div className="loading-state">
                        <div className="spinner"></div>
                        <p>{t('Dashboard.loading') }</p>
                    </div>
                ) : searchResults.length === 0 ? (
                    <div className="empty-state glass">
                        <i className="ti ti-search-off"></i>
                        <p>{t('Collection.NoSearchResults')}</p>
                        <p className="empty-subtitle">
                            {t('Search.TryDifferentTerm')}
                        </p>
                    </div>
                ) : (
                    <div className="games-grid">
                        {searchResults.map(game => (
                            <Link
                                key={game.id}
                                to={`/Game/${game.id}`}
                                className="game-card"
                            >
                                <div className="game-card-image">
                                    {imageErrors[game.id] ? (
                                        <div className="image-fallback">
                                            <i className="ti ti-device-gamepad-2"></i>
                                        </div>
                                    ) : (
                                        <img
                                            src={game.image}
                                            alt={game.name}
                                            onError={() => handleImageError(game.id)}
                                            loading="lazy"
                                        />
                                    )}
                                </div>
                                <div className="game-card-info">
                                    <h3 className="game-card-title">{game.name}</h3>
                                    <div className="game-card-meta">
                                        <span className="app-id">App ID: {game.id}</span>
                                    </div>
                                </div>
                            </Link>
                        ))}
                    </div>
                )}
            </div>
        </main>
    );
}