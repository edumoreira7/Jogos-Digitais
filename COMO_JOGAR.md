# Elemental Quest

Abra `Assets/Scenes/Inicio.unity` e pressione Play no Unity.

- A / D ou setas: andar.
- Shift: correr.
- Espaco: pular.
- J: lancar magia.
- Esc: pausar ou continuar.

## Objetivo

Recupere os 16 cristais do Jardim das Nuvens e entre no portal para chegar as Ruinas Esmeralda. Recupere os 20 cristais restantes e entre no ultimo portal para vencer. Alguns cristais ficam sobre plataformas suspensas. E possivel voltar para buscar os que faltam. As 30 pecas do tileset foram recortadas e utilizadas em cada fase, incluindo rampas, ilhas curvas, bordas, vegetacao, flores e pedras.

Cada cristal vale 100 pontos. Derrotar um inimigo com magia vale 50 pontos. O guaxinim patrulha e ataca de perto; o pato verde dispara sementes magicas; o sapo guardiao e maior e exige dois acertos. Todos possuem quadros de movimento, ataque, dano e morte retirados das cartelas fornecidas. As tres vidas sao compartilhadas entre as duas fases. Encostar em um inimigo, receber um disparo ou cair em um abismo custa uma vida. Apos sofrer dano, o pato fica temporariamente invulneravel. Uma queda reposiciona o pato no inicio da fase sem apagar os cristais coletados. Ao perder as tres vidas, aparece a tela de derrota. Reiniciar comeca uma nova partida com pontuacao zerada e tres vidas.

## Cenas

- `Inicio`: instrucoes e botao de jogar.
- `Fase1`: Jardim das Nuvens, com 16 cristais e tres inimigos.
- `Fase2`: Ruinas Esmeralda, com 20 cristais e quatro inimigos.
- `Final`: vitoria ou derrota, pontuacao, reiniciar e voltar ao inicio.
- `SampleScene`: cena original preservada para referencia.

Os recortes ficam em `Assets/ElementalQuest/Recortes`. Expanda Cenario, Guaxinim, Atirador e Guardiao na janela Project para reutiliza-los. O arquivo CATALOGO.txt lista os nomes. Os PNGs originais e seus recortes manuais foram preservados. Rampas e ilhas usam colisores poligonais obtidos do contorno da imagem; plantas e pedras decorativas nao bloqueiam o caminho. O parallax acompanha somente o deslocamento horizontal da camera: se ela parar nos limites da fase, o fundo tambem para, mesmo que o pato continue andando. O jogador reutiliza o controlador, os sprites, as animacoes e o projetil originais.

## Edicao

O guaxinim usa um multiplicador de velocidade de 1,7 (cerca de 70% mais rapido). O sapo guardiao detecta o jogador a ate 8 unidades e persegue a 2,2 unidades por segundo quando ambos estao no mesmo trecho continuo de chao. Ele respeita as bordas, para para atacar e retorna gradualmente a sua patrulha quando perde o alcance. Esses valores ficam no componente QuestEnemy. O guardiao nao pula abismos nem sobe em plataformas separadas.

As fases contem objetos editaveis na Hierarchy: ilhas, plataformas, cristais, inimigos e portal. Ao adicionar ou remover cristais, atualize `Total Itens` no objeto `Regras, vidas e pontuacao`. Os limites de patrulha, o tipo e as vidas de cada inimigo ficam no componente `QuestEnemy`.

O menu `Elemental Quest > Criar ou reconstruir aventura` recria as quatro cenas e sobrescreve edicoes feitas nelas. Use somente para voltar ao layout inicial desta aventura.

## Seguranca

A branch `backup/plataforma-2026-10-16` guarda o estado anterior a esta aventura no repositorio `edumoreira7/Jogos-Digitais`. A implementacao fica na branch `elemental-quest/duas-fases`.
