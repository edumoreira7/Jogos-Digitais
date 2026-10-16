# Elemental Quest

Abra `Assets/Scenes/Inicio.unity` e pressione Play no Unity.

- A / D ou setas: andar.
- Shift: correr.
- Espaco: pular.
- J: lancar magia.
- Esc: pausar ou continuar.

## Objetivo

Recupere os 16 cristais do Jardim das Nuvens e entre no portal para chegar as Ruinas Esmeralda. Recupere os 20 cristais restantes e entre no ultimo portal para vencer. Alguns cristais ficam sobre plataformas suspensas. E possivel voltar para buscar os que faltam.

Cada cristal vale 100 pontos. Derrotar um slime com magia vale 50 pontos. As tres vidas sao compartilhadas entre as duas fases. Encostar em um slime ou cair em um abismo custa uma vida. Apos sofrer dano, o pato fica temporariamente invulneravel. Uma queda reposiciona o pato no inicio da fase sem apagar os cristais coletados. Ao perder as tres vidas, aparece a tela de derrota. Reiniciar comeca uma nova partida com pontuacao zerada e tres vidas.

## Cenas

- `Inicio`: instrucoes e botao de jogar.
- `Fase1`: Jardim das Nuvens, com 16 cristais e tres slimes.
- `Fase2`: Ruinas Esmeralda, com 20 cristais e quatro slimes.
- `Final`: vitoria ou derrota, pontuacao, reiniciar e voltar ao inicio.
- `SampleScene`: cena original preservada para referencia.

O chao e as plataformas usam os tiles de terra e vegetacao fornecidos. Os fundos novos acompanham o deslocamento horizontal do pato. Os slimes, cristais e portais possuem animacoes em quadros. O jogador reutiliza o controlador, os sprites, as animacoes e o projetil originais.

## Edicao

As fases contem objetos editaveis na Hierarchy: ilhas, plataformas, cristais, slimes e portal. Ao adicionar ou remover cristais, atualize `Total Itens` no objeto `Regras, vidas e pontuacao`. Os limites de patrulha de cada slime ficam no componente `QuestEnemy`.

O menu `Elemental Quest > Criar ou reconstruir aventura` recria as quatro cenas e sobrescreve edicoes feitas nelas. Use somente para voltar ao layout inicial desta aventura.

## Seguranca

A branch `backup/plataforma-2026-10-16` guarda o estado anterior a esta aventura no repositorio `edumoreira7/Jogos-Digitais`. A implementacao fica na branch `elemental-quest/duas-fases`.
