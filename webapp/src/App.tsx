import { useEffect, useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'

function App() {
  const [count, setCount] = useState(0)
  const [socket, setSocket] = useState<WebSocket | null>(null)
  const [messages, setMessages] = useState<string[]>([])

  useEffect(() => {
    const ws = new WebSocket("ws://localhost:8080");

    ws.onopen = () => console.log("Conexao aberta");
    ws.onmessage = (e) => {
      const newMessage = `[${(new Date).toLocaleTimeString()}]: ${e.data}`;
      setMessages((prevMessages) => [...prevMessages.slice(-10), newMessage]);
    };
    ws.onclose = () => console.log("Conexão encerrada");

    setSocket(ws);

    return () => {
      ws.close();
    }
  }, [])

  const enviaMsg = () => {
    socket?.send("ola :D");
  }

  return (
    <>
      <div>
        <a href="https://vitejs.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>Vite + React</h1>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>
        <button
          onClick={() => enviaMsg()}
        >
          Envia mensagem para servidor
        </button>
        <p>
          Edit <code>src/App.tsx</code> and save to test HMR
        </p>
      </div>
      <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p>

      <h2 className="read-the-docs">
        As seguintes mensagens foram recebidas pelo websocket:
      </h2>
      <ul>
        {messages.map((message, index) => (
          <li key={index}>{message}</li>
        ))}
      </ul>
    </>
  )
}

export default App
