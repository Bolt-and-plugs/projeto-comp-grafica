### introdução

é aplicado tanto de maneira estática quanto dinâmica
- cenas estáticas -> foca em fotorealismo
- cenas dinâmicas -> foca em uma evolução/animação natural

em filmes, o tempo gasto computacionalmente não é relavante, diferente de em jogos e simuladores de voo

existe o balanço entre qualidade e eficiência, que é definido pelo contexto da aplicação

existem 10 tipos de nuvens:
![[fig1 - catadao.png]]

podem também ser classificadas com base em:
- elevação (baixo, médio ou alto)
- conteúdo (água ou gelo)
- estrutura (em camadas ou dispersa)

### modelamento

existem várias técnicas para criara formas semelhantes à nuvens reais, entre elas temos as *mesh-based* e métodos volumétricos, gernado formas a partir de texturas ou ruído (até mesmo a partir de imagens)

**textura**

método de Gardner
- usa elipses para modelar nuvens tridimensionais grosseiras e aplica series de Fourier para modelar detalhes das nuvens, nível de sombreamento e translucidez do céu
- para cria as camadas de nuvens, os mesmos parâmetros eram aplicados em uma textura de plano único

método de Elinas e Stuerzlinger
- novidade -> primitivas elipsoidais
- controle das propriedades de textura (como a transparência) para simular a irregularidade
- "mix de balões para criar uma forma complexa e difusa"

método de Ebert e Parent
- aplicavam a textura ao volume inteiro do objeto (ao inves de apenas à superfície) -> texturização sólida de malhas poligonais
- a transparência controlada gerava a forma de substância gasosa
- "ao invés de pintar a superfície de uma esfera, preencha ela com um gradiente de fumo, controlando a densidade"

"extras"
- cálculo de opacidade com algoritmos fractais
- projeto de simulação da atmosfera de júpiter

**ruído**

sakas -> síntese espectral e a teoria da turbulência
Xu -> simulam usando autômatos celulares modificados, usam campos de propabilidade estotástica (aleatória)
Webank -> criam diferentes nuvens usando variações de funções de densidade e de forma
Neyret -> usa um shader que cria um efeito volumétrico adicionando Perlin noise na superfície da nuvem
Goswami -> mapa de nuvens que é uma textura de ruído pré-calculada usada para governar as formas das nuvens e melhorar a eficiência da renderização

**geométrica**

Ebert
- usa uma função implícita volumétrica (equação matemática que define uma forma) para gerar nuvens cumulus

Schpok
- desenvolveu uma estrutura de modelagem que permite o usuário interagir diretamente para definir a forma da nuvem usando elipsoides e adicionar detalhes com filtros de ruído

autômatos celulares
- é uma grade de células, onde cada célula muda de estado com base em regras que dependem de seus vizinhos e do tempo

**formas**

hierarquia de gotas 
- modelam se baseando em gotas
- sobreposição de gotas filhas sobre gotas pai

esboço e B-spline
- pode-se utilizar uma superfície B-spline (curva paramétrica) 
- a partir de esboços 2D, infere-se um esqueleto e coloca esferas por ele -> Wither
- utiliza um esboço para gerar uma malha de nuvem, depois preenche-se com partículas diferentes (espefícios para cada tipo de nuvem)

Yu e Wang
- partem de formas 2D ou 3D para gerar nuvens
- quando usam 2D, elas são esticadas para 3D e um eixo medial é extraido para ajudar no processo de amostragem
- permitem o morfismo entre tipos de nuvens

**baseado em imagens**

reconhecimento de padrões e classificação
- Peng e Cheng abordaram como um problema de rotulação 

reconstrução a partir de imagem única
- Yuan et al
![[fig 3 - catadao.png]]

**imagens de satélite**

não é muito necessário se prolongar nesse...







