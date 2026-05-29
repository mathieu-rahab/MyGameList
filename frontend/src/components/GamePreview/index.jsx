import './index.css';
import {useUserService} from "../../api/userService.js";

export default function GamePreview({ game }) {
  return (
    
    <div className="scard glass">
      <img src={game.image} alt={`Couverture de ${game.name}`} className="game-cover" />
      {/* L'overlay qui contient les infos superposées */}
      <div className="game-info">
        {/* Les infos en bas */}
        <div className="game-details">
          <h3 className="game-title">{game.name}</h3>
        </div>
        
      </div>
    </div>
  );
}

