import './index.css'; // Crée ce fichier CSS dans le même dossier

export default function GamePreview({ game }) {
  return (
    <div className="game-card">
      <img src={game.coverUrl} alt={`Couverture de ${game.title}`} className="game-cover" />
      
      {/* L'overlay qui contient les infos superposées */}
      <div className="game-info">
        
        {/* Les badges en haut */}
        {(game.category || game.version) && (
          <div className="game-badges">
            {game.category && <span className="badge category">{game.category}</span>}
            {game.version && <span className="badge version">{game.version}</span>}
          </div>
        )}

        {/* Les infos en bas */}
        <div className="game-details">
          <h3 className="game-title">{game.title}</h3>
          <div className="game-meta">
            {game.year && <span>📅 {game.year}</span>}
            {game.size && <span>💾 {game.size}</span>}
          </div>
        </div>
        
      </div>
    </div>
  );
}