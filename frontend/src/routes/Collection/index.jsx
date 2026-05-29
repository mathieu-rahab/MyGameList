
import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { useTranslation } from "react-i18next";
import "./index.css";
import { useUserService } from "../../api/userService.js";
import { useAuth } from "../../utils/useAuth.jsx";

export default function Collection() {
    const { t } = useTranslation();
    const [searchParams] = useSearchParams();
    const { user } = useAuth();
    const { getOneCollection, searchGames, addGameCollection, removeGameCollection } = useUserService();

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

                // For now, simulate game data since we don't have the actual API
                // In a real implementation, you would fetch game details for each game in the collection
                const mockGames = collection.gamesId.map(id => ({
                    appId: id,
                    name: `Game ${id}`,
                    tinyImage: `test ...`
                }));
                setGames(mockGames);
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
    }, [searchParams, user]);

    // Handle search with debounce
    useEffect(() => {
        const timeoutId = setTimeout(() => {
            if (searchTerm.trim()) {
                handleSearch(searchTerm);
            } else {
                setSearchResults([]);
            }
        }, 500);

        return () => clearTimeout(timeoutId);
    }, [searchTerm]);

    // Handle search with real API call
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

    // Handle adding a game to the collection
    const handleAddGame = async (game) => {
        try {
            // Get the add-game link from collection
            const addGameCollectionLink = collection.links?.find(
                link => link.rel === 'add-game'
            );

            if (!addGameCollectionLink) {
                console.error("Add game link not found");
                return;
            }

            // Call the API to add the game
            await addGameCollection(addGameCollectionLink.href, game.appId);

            // Update the local state to include the new game
            setGames(prevGames => [...prevGames, game]);

            // Remove the game from search results
            setSearchResults(prevResults =>
                prevResults.filter(result => result.appId !== game.appId)
            );
        } catch (err) {
            console.error("Error adding game to collection:", err);
        }
    };

    const GameRow = ({ game, showRemoveButton = false }) => (
        <div className="game-row">
            <div className="gthumb">
                <img src={game.tinyImage} alt={game.name} />
            </div>
            <div className="ginfo">
                <div className="gname">{game.name}</div>
                <div className="gmeta">App ID: {game.appId}</div>
            </div>
            <div className="gactions">
                {showRemoveButton ? (
                    <button className="btn btn-secondary">
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

                    <div className="search-box">
                        <input
                            type="text"
                            placeholder={t('Collection.SearchPlaceholder')}
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            className="form-control"
                        />
                        <button className="btn btn-primary" onClick={() => handleSearch(searchTerm)}>
                            <i className="ti ti-search"></i>
                        </button>
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