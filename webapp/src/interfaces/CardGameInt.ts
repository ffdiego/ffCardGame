export interface IEstadoTelaDAO {
    Estado: string;
    NumeroJogadores: number;
    OutrosJogadores?: IJogador[];
    CartasMesa?: ICarta[];
}

export interface IJogador {
    Nome: string;
    Mao: ICarta[];
    Saldo: number;
}

export interface ICarta {
    Naipe: number;
    Numero: number;
}