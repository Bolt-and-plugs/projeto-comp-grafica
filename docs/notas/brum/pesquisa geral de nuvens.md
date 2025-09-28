### 0 - introdução
as nuvens são fenômenos naturais "amorfos e dinâmicos"

é necessário simular como a aluz interage com esse meio

### 1 - fundamentos físicos e modelagem do caos

#### 1.1 modelo óptico de volumes participativos
as nuvens são um exemplo de "meio participativo", isto é, um volume de partículas que interage com a luz à medida que esta o atravessa
as interações podem ser:
- emissão
- absorção
- dispersão (*scattering*) -> forma mais relevante para as nuvens

a dispersão ocorre predominantemente através de dois mecanismos físicos
- dispersão de Rayleigh
- dispersão de Mie

##### dispersão de rayleigh 

é **causada por partículas muito pequenas** cujo **diâmetro é inferior a um décimo do comprimento de onda da luz visível**

por conta da dependência do comprimento de onda (já para a frequência da luz, quanto maior, mais fácil é a dispersão), é mais eficaz para ondas curtas (azuis) do espectro visível, resultando na luz azul do sol espalhada em todas as direções pela atmosfera. já no por do sol, a luz vermelha (comprimento mais longo) atravessam a atmosfera mais densa (menos dispersão e percorrendo maiores distâncias) resultando em um tom avermelhado no nascer/por do sol

##### dispersão de mie

causada por partículas maiores, como as gotículas de água e os cristais de gelos que compõe as nuvens e nevoeiro. o diâmetro dessas partículas é comparável ou maior que o comprimento de onda da luz visível

essa dispersão não é fortemente dependende do comprimento de onda

as gotículas de nuvem espalham todos os comprimentos de onda da luz visível de forma igual, quando a luz solar (combinação de todas as cores) é espalhada igualmente, resulta na luz branca, e quando em volumes muitos densos, são cinzentas, pois grande parte da luz que as atravessa é igualmente dispersada e re-direcionada, e quando a luz é completamente bloqueada, parecem escuras

| Característica                     | Dispersão de Rayleigh           | Dispersão de Mie                    |
| ---------------------------------- | ------------------------------- | ----------------------------------- |
| Partículas causadoras              | moléculas de ar, aressóis finos | gotículas de água, cristais de gelo |
| Tamanho das partícuals             | pequeno                         | grande                              |
| Dependência do Comprimento de Onda | forte                           | fraca                               |
| Exemplos                           | céu azul, nascer/por do sol     | nuvens brancas, nevoeiro            |

#### 1.2 modelagem de nuvens: da simulação à geração procedimental

a tradução dos princípios físicos para a forma digital requer me´todos de modelagfem que captura a anatureza fluida das nuvens.

uma abordagem é a **Modelagem Baseada em Física**, que se baseia na **Dinâmica de Fluido Computacional (CFD)**.
utiliza equações (como a de Navier-Stokes) para simular o movimento de fluidos (liquidos ou gases), ela é extremamente custosa computacionalmente.

outra abordagem é a **Geração Procedimental**, que cria nuvens atraǘes de "funções pseudoaleatórias".
foi desenvolvido o ruído de Ker Perlin, gerando texturas com uma aparência de "aleatoridade controlada".
o ruído de Perlin e seus sucessores, como o ruído fractal, sãousados para modelas a densidade e forma das nuvens eficientemente.

### 2 - algoritmos de renderização em tempo real e otimizações essenciais

#### 2.1 a marcha de raios volumétrica (*volumetric ray marching*): o padrão ouro

método dominante para a renderização de nuvens volumétricas em tempo real.

princípio básico: para cada pixel na tela, um raio é lançado da câmara para o volume da nuvem. esse raio é então percorrido em etapas discretas, a cada passo, o algoritmo coleta amostras de densidade e cor do volume, que sõa acumuladas para detemrinar a cor final do pixel (densidade = combinação de funções de ruído, como Perlin e Worley).

complexidade é $O(n²)$, em que $n$ é o número de amostras por raio. 

para otimizar este processo, tem a **Marcha de Raios Desacoplada** que reduz para $O(n \space log \space n)$, que separa o cálculo da dispersão interna da transmitância e usa amostragem por importância, focando o esforço computacional apenas nos segmentos de raio que contribuem mais.

outra otimização é o **sampling adaptativo**, que ajusta o tamanho do passo da "marcha" para ser maior em áreas vazias e menor em áreas densas.

#### 2.2 o princípio das abordagens híbridas e de pré-computação

algoritmo de duas passagens de Mark Harris:

- **1° passo (pré-processamento):** a primeira passagem pré-calcula a iluminação complexa da nuvem (dispersão múltipla de luz para a frente na direção da fonte de luz). a ausência de dispersão múltiplca, faz com que as nuvens pareçam escuras ao usar valores realistas de profundidade óptica.

- **2° passo (tempo real):** utilizada os dados do 1° passo para renderizar rapidamente a dispersão simples em direção ao ponto de vista do observador. 

outras otimizações como **impostores**, em que nuvens distantes - sem detalhes volumétrico completo - são "pinturas" em um plano distante, e também **sistemas de partículas**, onde não se usa uma abordagem volumétrica para adicionar detalhes locais, como fumo ou névoa

### 3 - estudos de caso: a realidade da renderização de nuvens na indústria

#### 3.1 horizon forbidden west: nuvens exploráveis

permitia que os jogadores voassem diretamente atraǘeis de formação de nuvens dinâmicas.

a solução foi um sistema baseado em *voxels* para a modelagem, interpretando dados de voxel de baixa resolução e adicionando detalhes durante a renderização.

#### 3.2 microsoft flight simulator: simulação planetária

focado na precisaõ da simulação, o jogo utiliza dados meteorológicos do mundo real, gerando nuvens dinâmicas em 32 camadas atmosféricas, permitindo que os jogadores voem através delas e observem como as forças atmosféricas interagem com a aeronave.

### 4 - conclusão

|Ano|Contribuinte/Evento|Significado|
|---|---|---|
|**1982**|Filme _Tron_ e Ken Perlin|O visual "maquinal" do CGI inspira Ken Perlin a criar o seu ruído, uma técnica de geração procedural para texturas orgânicas e complexas.|
|**1983**|Reeves|Introdução dos sistemas de partículas para modelar fenômenos "desfocados" (`fuzzy`) como nuvens.|
|**1985**|Ken Perlin|Artigo na SIGGRAPH ("An Image Synthesizer") descreve formalmente o Ruído de Perlin, que se tornaria uma técnica fundamental.|
|**1986**|James Kajiya|Formulação da "Equação de Renderização", a base para a iluminação global e renderização fisicamente precisa.|
|**1988**|Drebin et al., Levoy|Obras seminais que introduzem o conceito de "renderização volumétrica".|
|**2000s**|G-cluster, OnLive|Início do "cloud gaming" e da renderização em nuvem, que enfrenta desafios de latência.|
|**2002**|Mark Harris|Apresentação do algoritmo de duas passagens que aproxima a dispersão múltipla, tornando a renderização de nuvens viável para simuladores de voo.|
|**2015**|Guerrilla Games|Introdução do sistema Nubis em _Horizon Zero Dawn_, um marco para as nuvens volumétricas em jogos.|
|**2022**|Andrew Schneider|Apresentação do "Nubis, Evolved" na SIGGRAPH, mostrando as otimizações para permitir a exploração de nuvens em tempo real.|
|**Atual**|IA/Neural Rendering|Surgimento de novas abordagens que usam redes neurais para replicar pipelines de renderização inteiros sem computação gráfica tradicional.|

### 5 - referências

gemini deep research
https://developer.nvidia.com/gpugems/gpugems/part-vi-beyond-triangles/chapter-39-volume-rendering-techniques
https://en.wikipedia.org/wiki/Perlin_noise
https://en.wikipedia.org/wiki/Ray_marching
https://www.ansys.com/simulation-topics/what-is-computational-fluid-dynamics
https://rebusfarm.net/blog/what-is-cloud-rendering


---

