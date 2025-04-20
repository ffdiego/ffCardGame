import { useEffect, useState } from 'react'
import './App.css'
import { ICarta, IEstadoTelaDAO } from './interfaces/CardGameInt';
import { motion } from 'motion/react';

const URL = 'localhost:8080';
const BASEURL = 'http://' + URL;
const BASEWS = 'ws://' + URL;

function Carta({ naipe, numero, className, style }: 
  { naipe: number, numero: number, className?: string, style?: React.CSSProperties }) 
{
  const especiais: Record<number, string> = {
    1: 'A',
    10: 'T',
    11: 'J',
    12: 'Q',
    13: 'K'
  };
  const naipes: Record<number, string> = {
    0: 'B',
    1: 'D',
    2: 'S',
    3: 'H',
    4: 'C'
  };

  const escondida = (numero == 0);
  const [numeroCod, naipeCod] = escondida
    ? [1, 'B']
    : [especiais[numero] ?? numero, naipes[naipe]];

  return <img className={className} style={style} src={`playingcards/${numeroCod}${naipeCod}.svg`} />
}

function Cartas({ cartas, size = 8 }:
  {
    cartas?: ICarta[],
    size?: number,
    modo?: 'ladoalado' | 'sobrepostas'
  }) {
  
  const quantidadeCartas = (cartas?.length ?? 0);
  const sobrepoe = (quantidadeCartas > 2);
  const espacoMaximo = size * 24 * 2;
  const deslocamentoEsquerda = sobrepoe 
  ? ((quantidadeCartas * size * 24) - espacoMaximo) / (quantidadeCartas - 1) 
  : 0;

  return <div style={{ width: size * 48 }} className='flex justify-start transition-all'>
    {cartas?.map((carta, index) =>
    (
      <Carta
        style={{marginRight: -deslocamentoEsquerda, width: size * 24}}
        key={index}
        naipe={carta.Naipe} numero={carta.Numero} />
    ))}
  </div>
}

function App() {
  const [serverHealth, setServerHealth] = useState(false);
  const [socket, setSocket] = useState<WebSocket | null>(null)
  const [estadoTela, setEstadoTela] = useState<IEstadoTelaDAO>({
    Estado: ":D",
    CartasMesa: [
      { Numero: 2, Naipe: 1 },
      { Numero: 1, Naipe: 2 }
    ],
    OutrosJogadores: [
      {
        Nome: 'Diego',
        Saldo: 500.00,
        Mao: [
          { Numero: 13, Naipe: 1 },
          { Numero: 1, Naipe: 2 },
        ]
      },
      {
        Nome: 'Amiguinho',
        Saldo: 410.50,
        Mao: [
          { Numero: 10, Naipe: 4 },
          { Numero: 1, Naipe: 2 }
        ]
      }
    ],
    NumeroJogadores: 0,
  })

  useEffect(() => {
    const interval = setInterval(() => {
      fetch(BASEURL + '/health').then(
        (res) => {
          console.log(res);
          setServerHealth(res.ok)
        }
      )
    }, 10000);

    return () => clearInterval(interval);
  }, []);

  useEffect(() => {
    const ws = new WebSocket(BASEWS + '/assistir/0');

    ws.onopen = () => console.log("Conexao aberta");
    ws.onmessage = (e) => {
      const newMessage = `[${(new Date).toLocaleTimeString()}]: ${e.data}`;
      setMessages((prevMessages) => [...prevMessages.slice(-10), newMessage]);

      const obj = JSON.parse(e.data);
      if (obj?.tipo !== "EstadoTela") return;

      setEstadoTela(obj.conteudo as IEstadoTelaDAO);
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
      <h3 className={serverHealth ? 'text-green-400' : 'text-red-600'}>{serverHealth ? 'Conectado' : 'Não conectado'}</h3>
      <h1><b>Estado</b>: {estadoTela.Estado}</h1>
      <div className='border-2 rounded border-dashed border-white p-8 m-2 overflow-hidden'>
        <Cartas cartas={estadoTela.CartasMesa} size={10} />
      </div>
      <div className="card">
        <button
          onClick={() => enviaMsg()}
        >
          Envia mensagem para servidor
        </button>
        <button onClick={() => fetch(BASEURL + '/admin/0?acao=avanca')}>
          Avança
        </button>
      </div>

      <div className='flex'>
        {estadoTela.OutrosJogadores?.map(jogador =>
          <div>
            <div className='mx-4 flex justify-between text-2xl font-bold'>
              <p>{jogador.Nome}</p>
              <p className='bg-white rounded text-green-900 px-1'>R${jogador.Saldo.toFixed(2)}</p>
            </div>
            <div
              className='bg-green-950 border-2 rounded border-dashed border-white p-8 m-2 overflow-hidden text-2xl text-left'>
              <p>Aposta: </p>
              <Cartas cartas={jogador.Mao} size={6} />
            </div>
          </div>
        )}
      </div>
    </>
  )
}

export default App
