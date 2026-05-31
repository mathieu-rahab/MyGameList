import './index.css';

export default function GamePreview({ game }) {
  return (
    
    <div className="scard glass game-preview-container">
      <img src={game.headerImage } alt={`Couverture de ${game.name}`} className="game-cover" />
      {/* L'overlay qui contient les infos superposées */}
      <div className="game-info">
        {/* Les infos en bas */}
        <div className="game-details">
          <h1 className="game-title">{game.name}</h1>
        </div>
        
      </div>
    </div>
  );
}

