Referência: "An Image Synthetizer" de Ken Perlin: Nesse paper ele sugeriu sobre a técnica de "Pixel Streaming", como é um paper um pouco antigo, acredito que isso deva ter em toda engine que a gente for usar atualmente. Algumas fundamentações são bem parecidas com o que temos em OpenGL, com as funções espaciais (x,y) ou (x,y,z). //Comentários off topic.

//Tudo aqui comentado deve ser interpretado direcionando a aplicação para pixel.
## Mapeamento de Texturas

Qualquer função que apresente o domínio entre as dimensôes pode ser considerado uma "função espacial". A partir disso, cada função espacial pode ser interpretada como a representação de um material sólido.
Desta forma, ao avaliar estas funções nos pontos visíveis da superfície de um objeto, é possível obter a textura da superfície, de modo parecido a ter "contornado" o objeto. A textura obtida a partir deste tipo de extração
foi tratada como "textura sólida". Este termo foi usado posteriormente para a descrição da variedade de objetos e explicação de outros conceitos.

## Noise //uau o perlin noise

A princípio, o "Noise" foi definido como uma primitiva de modelagem de textura para o Pixel Streaming Editor que o autor propôs, mas a implementação da técnica pode ser reproduzida em outras ferramentas. 

Em um espaço vetorial onde cada ponto para as coordenadas x,y,z são inteiros, cada ponto deste conjunto pode ser associado a um "pseudorandom" valor, onde x,y, e z são valores gradientes.
Deste modo, deve-se mapear cada sequência ordenada de três inteiros em uma sequencia ordenada de 4 números reais, de modo que:

[a,b,c,d] = H([x,y,z]) onde [a,b,c,d] definem a equação com o gradiente [a,b,c] e o valor "d" em [x,y,z]. H() seria uma função HASH. 

A partir disso, se [x,y,z] está neste set de inteiros, é definido o Noise([x,y,z]) = d[x,y,z], interpretado como um sinal (apresenta frequência). 

Com estas definições, supondo um vetor aleatório array, e que ele representa um Donut 

color = white * Noise(array), com a aplicação desta "transformada" foi possível criar essas ondulações no donut com as texturas brancas, de forma aleatória.

<img width="392" height="288" alt="image" src="https://github.com/user-attachments/assets/6c8e230e-085e-4847-af3c-1380b8e5f99e" />

color = Colorful(Noise(k * array)) outro possível exemplo de aplicação, 

<img width="380" height="272" alt="image" src="https://github.com/user-attachments/assets/afdd513c-70ee-4efd-a93c-fb982e565e3e" />

<img width="382" height="272" alt="image" src="https://github.com/user-attachments/assets/a048363d-533e-4d24-abb5-a3314908bb5b" />

Outra técnica seria o vetor diferencial do Noise(), que é definido pela taxa de variação instantânea do "ruído" através das três direções, chamados de Dnoise().
A partir desta aplicação fica possível criar outros tipos de perturbação, originando novas superfícies e texturas. 

normal + = Dnoise(array) 

<img width="395" height="298" alt="image" src="https://github.com/user-attachments/assets/6420daf3-f461-41c9-9a3d-beaa517c8525" />


Como estes cálculos e tecnicas são aplicados em nível de pixel, a frequència de cada pixel é que está sendo utilizada, logo, qualquer frequência de taxa mais alta que não seja desejada é automaticamente removida. 

Exemplo de cálculo com a frequência inversa, aplicando transformações em todos os octetos:
f=l
while f < pixel_freq
normal + = Dnoise(f * point)
f*=2 

O autor descreve uma técnica para simular a aparência de mármore usando a função Noise(). O método parte do princípio de que a aparência do mármore resulta de camadas heterogêneas que foram deformadas por 
forças turbulentas antes de se solidificarem.
A abordagem é, portanto, uma combinação de uma estrutura regular e simples (as camadas) com uma complexa estrutura estocástica (o ruído da turbulência).
A base do modelo são as camadas, representadas por uma simples onda senoidal, sin(x). O autor usa a coordenada 
point[1] como o input para essa função, e o valor resultante é então mapeado para cores através de uma função auxiliar marble_color().
Para adicionar o realismo das forças turbulentas, o autor introduz uma função 
turbulence(). Esta função é usada para perturbar a coordenada de entrada 
x antes que ela seja passada para a onda senoidal. O pseudocódigo que combina esses elementos é o seguinte:

function marble(point)
  x = point[1] + turbulence(point)
  return marble_color(sin(x))
A função 

turbulence() é, por sua vez, uma soma de ruído em diferentes escalas, um processo que cria um padrão auto-semelhante ou 1/f. O algoritmo para a 

turbulence() é detalhado como:

function turbulence(p)
  t = 0
  scale = 1
  while (scale > pixelsize)
    t += abs(Noise(p/scale) * scale)
    scale *= 2
  return t
Este procedimento garante que a quantidade de ruído adicionada em cada escala seja proporcional ao seu tamanho, resultando na impressão visual de movimento browniano. Além disso, o uso da função 
abs() em cada iteração assegura que o gradiente da textura tenha limites descontínuos em todas as escalas, o que é interpretado visualmente como fluxo turbulento


<img width="705" height="800" alt="image" src="https://github.com/user-attachments/assets/2ed38e18-4609-45f3-a104-b5bcb622b71c" />

//As principais definições são essas, o autor deixa alguns exemplos com algoritmos como núvens e bolhas, fogo e água, utilizando essa função "turbulence" e algumas modificações;
