//routes management
import { BrowserRouter, Route, Routes } from "react-router";
import Home from "./routes/Home";
import Login from "./routes/Login";
import NewUser from "./routes/NewUser";
import Game from "./routes/Game";
import Dashboard from "./routes/Dashboard";

//layout
import Layout from "./components/Layout";

export default function App(){
  return (
    <BrowserRouter>
        <Routes>
            <Route element={<Layout/>}>
                <Route path="/" element= {<Home />}/>
                <Route path="/login" element={<Login />}/>
                <Route path="/newUser" element={<NewUser />}/>
                <Route path="/Game" element={<Game/>}/>
                <Route path="/dashboard" element={<Dashboard/>}/>
            </Route>
        </Routes>
    </BrowserRouter>
);
}