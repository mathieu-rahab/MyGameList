import { useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { useTranslation } from "react-i18next";
import "./index.css";
import { useUserService } from "../../api/userService.js";
import { useAuth } from "../../utils/useAuth.jsx";

export default function Collection() {
    const { t, i18n } = useTranslation();
    const [searchParams] = useSearchParams();
    const { user } = useAuth();
    const { getOneCollection, searchGames, addGameCollection, removeGameCollection, getGameInfo } = useUserService();

    const [collection, setCollection] = useState(null);
    const [games, setGames] = useState([]);
    const [searchResults, setSearchResults] = useState([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // Fetch collection details
    useEffect(() => {
        const fetchCollection = async () => {
            try {
                setLoading(true);
                setError(null);

                const userId = user?.userId;
                if (!userId) {
                    setError("User not found");
                    return;
                }

                const collectionId = searchParams.get('id');
                if (!collectionId) {
                    setError("Collection ID not found");
                    return;
                }

                // Get collections link from user
                const collectionsLink = user.links?.find(
                    link => link.rel === 'collections'
                );

                if (!collectionsLink) {
                    setError("Collections link not found");
                    return;
                }

                // Fetch all collections
                const collection = await getOneCollection(collectionsLink.href, collectionId);

                if (!collection) {
                    setError("Collection not found");
                    return;
                }

                setCollection(collection);

                if (collection.gamesId && collection.gamesId.length > 0) {
                    const gameDetails = await Promise.all(
                        collection.gamesId.map(async (gameId) => {
                            try {
                                const gameInfo = await getGameInfo(gameId, i18n.language);
                                return {
                                    appId: gameInfo.id,
                                    name: gameInfo.name,
                                    tinyImage: gameInfo.image
                                };
                            } catch (err) {
                                console.error(`Error fetching game info for ${gameId}:`, err);
                                // Fallback to minimal data if API fails
                                return {
                                    appId: gameId,
                                    name: `Game ${gameId}`,
                                    tinyImage: '/covers/placeholder.png'
                                };
                            }
                        })
                    );
                    setGames(gameDetails);
                } else {
                    setGames([]);
                }
            } catch (err) {
                console.error("Error fetching collection:", err);
                setError(err.message || "Failed to fetch collection");
            } finally {
                setLoading(false);
            }
        };

        if (user?.userId) {
            fetchCollection();
        }
    }, [searchParams, user, i18n.language]);

    // Recherche
    useEffect(() => {
        const timeoutId = setTimeout(() => {
            if (searchTerm.trim()) {
                handleSearch(searchTerm);
            } else {
                setSearchResults([]);
            }
        }, 400);

        return () => clearTimeout(timeoutId);
    }, [searchTerm]);

    const handleSearch = async (term) => {
        if (!term.trim()) {
            setSearchResults([]);
            return;
        }

        try {
            const results = await searchGames(term);
            setSearchResults(results);
        } catch (err) {
            console.error("Search error:", err);
            setSearchResults([]);
        }
    };

    // Gérer l'ajout d'un jeu à la collection
    const handleAddGame = async (game) => {
        try {
            const addGameCollectionLink = collection.links?.find(
                link => link.rel === 'add-game'
            );

            if (!addGameCollectionLink) {
                console.error("Add game link not found");
                return;
            }

            await addGameCollection(addGameCollectionLink.href, game.appId);
            setGames(prevGames => [...prevGames, game]);

            // Supprimer le jeu des résultats de recherche
            setSearchResults(prevResults =>
                prevResults.filter(result => result.appId !== game.appId)
            );
        } catch (err) {
            console.error("Error adding game to collection:", err);
        }
    };

    // Gérer la suppression d'un jeu de la collection
    const handleRemoveGame = async (game) => {
        try {
            // Get the remove-game link from collection
            const removeGameCollectionLink = collection.links?.find(
                link => link.rel === 'remove-game'
            );

            if (!removeGameCollectionLink) {
                console.error("Remove game link not found");
                return;
            }

            // Appelez l'API pour supprimer le jeu
            await removeGameCollection(removeGameCollectionLink.href, game.appId);

            setGames(prevGames => prevGames.filter(g => g.appId !== game.appId));

            // Réafficher le jeu dans les résultats de recherche
            setSearchResults(prevResults => [...prevResults, game]);
        } catch (err) {
            console.error("Error removing game from collection:", err);
        }
    };

    const GameRow = ({ game, showRemoveButton = false }) => (
        <div className="game-row">
            <div className="gthumb">
                <Link to={`/Game/${game.appId}`}>
                    <img src={game.tinyImage} alt={game.name} />
                </Link>
                
            </div>
            <div className="ginfo">
                <div className="gname">{game.name}</div>
                <div className="gmeta">App ID: {game.appId}</div>
            </div>
            <div className="gactions">
                {showRemoveButton ? (
                    <button className="btn btn-delete" onClick={() => handleRemoveGame(game)}>
                        <i className="ti ti-trash"></i>
                    </button>
                ) : (
                    <button className="btn btn-primary" onClick={() => handleAddGame(game)}>
                        <i className="ti ti-plus"></i>
                    </button>
                )
                }
            </div>
        </div>
    );

    if (loading) {
        return (
            <main className="collection-page">
                <div className="message">{t('Dashboard.loading')}</div>
            </main>
        );
    }

    if (error) {
        return (
            <main className="collection-page">
                <div className="message error">{error}</div>
            </main>
        );
    }

    return (
        <main className="collection-page">
            <div className="collection-header">
                <h1>{collection?.label || t('Collection.Untitled')}</h1>
                <div className="collection-meta">
                    {games.length} {t('Collection.GamesCount')}
                </div>
            </div>

            <div className="collection-content">
                <div className="section glass">
                    <div className="sec-head">
                        <span className="sec-title">{t('Collection.Games')}</span>
                    </div>

                    {games.length > 0 ? (
                        <div className="games-list">
                            {games.map(game => (
                                <GameRow
                                    key={game.appId}
                                    game={game}
                                    showRemoveButton={true}
                                />
                            ))}
                        </div>
                    ) : (
                        <div className="message">{t('Collection.NoGames')}</div>
                    )}
                </div>

                <div className="section glass">
                    <div className="sec-head">
                        <span className="sec-title">{t('Collection.AddGame')}</span>
                    </div>

                    <div className="input-wrap">
                        <i className="ti ti-search" aria-hidden="true"></i>

                        <input
                            type="text"
                            placeholder={t('Collection.SearchPlaceholder')}
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>

                    {searchResults.length > 0 && (
                        <div className="search-results">
                            <h3>{t('Collection.SearchResults')}</h3>
                            {searchResults.map(game => (
                                <GameRow key={game.appId} game={game} />
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </main>
    );
}