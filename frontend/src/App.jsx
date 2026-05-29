//routes management
import { BrowserRouter, Route, Routes } from "react-router";
import Home from "./routes/Home";
import Login from "./routes/Login";
import NewUser from "./routes/NewUser";
import Game from "./routes/Game";
import Dashboard from "./routes/Dashboard";
import Settings from "./routes/Settings";

//layout
import Layout from "./components/Layout";
import ProtectedRoute from "./components/ProtectedRoute.jsx";


export default function App(){
  return (
    <BrowserRouter>
        <Routes>
            <Route element={<Layout/>}>
                {/* Routes publiques */}
                <Route path="/" element= {<Home />}/>
                <Route path="/login" element={<Login />}/>
                <Route path="/newUser" element={<NewUser />}/>
                <Route path="/Game/:gameId" element={<Game/>}/>

                {/* Routes protégées */}
                <Route
                    path="/dashboard"
                    element={
                        <ProtectedRoute>
                            <Dashboard />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="/settings"
                    element={
                        <ProtectedRoute>
                            <Settings />
                        </ProtectedRoute>
                    }
                />
            </Route>
        </Routes>
    </BrowserRouter>
);
}