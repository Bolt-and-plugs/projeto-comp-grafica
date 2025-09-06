documentos a serem analisados:

[particle-based fluid sim](https://matthias-research.github.io/pages/publications/sca03.pdfhttps://matthias-research.github.io/pages/publications/sca03.pdf). 

[Real-Time Fluid Dynamics for Games](https://www.dgp.toronto.edu/public_user/stam/reality/Research/pdf/GDC03.pdf)
[gpu-gems-water](https://developer.nvidia.com/gpugems/gpugems/part-vi-beyond-triangles/chapter-38-fast-fluid-dynamics-simulation-gpu)

# Simulação de Fluidos em Computação Gráfica

Os maiores desafios da simulação de fluidos está nos aspectos fisicos que se aplicam, como por exemplo, convecção, difusão, turbulência e tensão superficial \cite{1}. No entanto, essas simulações eram (ao menos em 2003) quase inviáveis para serem empregadas em tempo real, portanto a precisão acaba sendo deixada parcialmente de lado nestas simulações.

A simulação de fluidos começou, basicamente, com a equação de de Navier-Stokes que descrevem a dinamica dos fluidos (não vamos nos aprofundar, mas para efeito de curiosidade, esse sistema de equações diferenciais se baseia em derivadas parciais e permitem determinar os campos de velocidade e de pressão num escoamento de fluidos). Sua forma geral é dada por:


$$
\rho \frac{D\mathbf{v}}{Dt} = -\nabla p + \mu \nabla^2 \mathbf{v} + \rho \mathbf{f}
$$


Uma explicação simples: a equação de Navier-Stokes descreve como o movimento de um fluido é influenciado pela pressão, viscosidade e forças externas. Nela, $\rho$ é a densidade do fluido, $\mathbf{v}$ é o vetor velocidade, $p$ é a pressão, $\mu$ é o coeficiente de viscosidade, e $\mathbf{f}$ representa forças externas (como gravidade). Ela permite calcular como o fluido se move e se comporta em diferentes situações.

[Para quem quiser se aprofundar](https://pt.wikipedia.org/wiki/Equa%C3%A7%C3%B5es_de_Navier-Stokes).

a technique for modeling a class of fuzzy objects. Since then
both, the particle-based Lagrangian approach and the grid-
based Eulerian approach have been used to simulate fluids
in computer graphics. Desbrun and Cani2 and Tonnesen22
use particles to animate soft objects. Particles have also been
used to animate surfaces7 , to control implicit surfaces23 and
to animate lava flows20 . In recent years the Eulerian ap-
proach has been more popular as for the simulation of fluids
in general18 , water 4, 3, 21 , soft objects 14 and melting effects1 .

Em 1983, T. Reeves \cite{reeves1983} introduziu sistemas de partículas como uma técnica para modelar uma classe de objetos difusos. Desde então, tanto a abordagem Lagrangiana baseada em partículas quanto a abordagem Euleriana baseada em grades têm sido usadas para simular fluidos em computação gráfica. Desbrun e Cani \cite{desbrun1996} e Tonnesen \cite{tonnesen1998} utilizam partículas para animar objetos macios. As partículas também foram usadas para animar superfícies \cite{witkin1991}, controlar superfícies implícitas \cite{bloomenthal1997} e animar fluxos de lava \cite{carlson2002}. Nos últimos anos, a abordagem Euleriana tem sido mais popular para a simulação de fluidos em geral \cite{fedkiw2001}, água \cite{stam1999, foster1996, enright2002}, objetos macios \cite{muller2002} e efeitos de derretimento \cite{carlson2002}.

A proposta deste artigo é simular fluidos com uma abordagem baseada  em Hidrodinamica de particulas lisinha KKKKKKKK (Smoothed Particle Hydrodynamics - SPH). 

### Resumo do artigo de Müller et al. (2003)

O artigo de Müller et al. (2003) \cite{muller2003} apresenta uma abordagem eficiente para simulação de fluidos baseada em Smoothed Particle Hydrodynamics (SPH). O método SPH representa o fluido como um conjunto de partículas, onde cada partícula carrega propriedades como massa, posição, velocidade e densidade. As interações entre partículas são calculadas usando funções de suavização (kernels), permitindo simular efeitos como pressão, viscosidade e forças externas.

As principais fórmulas utilizadas no SPH são:

**Densidade:**
$$
\rho_i = \sum_j m_j W(|\mathbf{r}_i - \mathbf{r}_j|, h)
$$
onde $\rho_i$ é a densidade da partícula $i$, $m_j$ é a massa da partícula $j$, $W$ é o kernel de suavização e $h$ é o raio de influência.

**Pressão:**
$$
\mathbf{f}_i^{\text{pressão}} = -\sum_j m_j \frac{p_i + p_j}{2 \rho_j} \nabla W(|\mathbf{r}_i - \mathbf{r}_j|, h)
$$
onde $p_i$ e $p_j$ são as pressões das partículas $i$ e $j$.

**Viscosidade:**
$$
\mathbf{f}_i^{\text{visc}} = \mu \sum_j m_j \frac{\mathbf{v}_j - \mathbf{v}_i}{\rho_j} \nabla^2 W(|\mathbf{r}_i - \mathbf{r}_j|, h)
$$
onde $\mu$ é o coeficiente de viscosidade e $\mathbf{v}_i$, $\mathbf{v}_j$ são as velocidades das partículas.

Essas fórmulas permitem calcular as forças que atuam sobre cada partícula, resultando em simulações de líquidos realistas e eficientes para aplicações interativas.




## Referências

```bibtex
@article{reeves1983,
	author = {Reeves, William T.},
	title = {Particle Systems—a Technique for Modeling a Class of Fuzzy Objects},
	journal = {ACM Transactions on Graphics},
	year = {1983},
	volume = {2},
	number = {2},
	pages = {91-108}
}
@inproceedings{desbrun1996,
	author = {Desbrun, Mathieu and Cani, Marie-Paule},
	title = {Smoothed particles: A new paradigm for animating highly deformable bodies},
	booktitle = {Eurographics Workshop on Computer Animation and Simulation},
	year = {1996}
}
@phdthesis{tonnesen1998,
	author = {Tonnesen, Dan},
	title = {Dynamically coupled particle systems for geometric modeling, reconstruction, and motion simulation},
	school = {University of Copenhagen},
	year = {1998}
}
@inproceedings{witkin1991,
	author = {Witkin, Andrew and Kass, Michael},
	title = {Reaction-diffusion textures},
	booktitle = {SIGGRAPH '91},
	year = {1991}
}
@book{bloomenthal1997,
	author = {Bloomenthal, Jules},
	title = {Introduction to Implicit Surfaces},
	publisher = {Morgan Kaufmann},
	year = {1997}
}
@inproceedings{carlson2002,
	author = {Carlson, Mark and Mucha, Peter J. and Van Horn, Robert and Metaxas, Dimitris},
	title = {Melting and Flowing},
	booktitle = {ACM SIGGRAPH/Eurographics Symposium on Computer Animation},
	year = {2002}
}
@inproceedings{fedkiw2001,
	author = {Fedkiw, Ronald and Stam, Jos and Jensen, Henrik Wann},
	title = {Visual Simulation of Smoke},
	booktitle = {SIGGRAPH 2001},
	year = {2001}
}
@inproceedings{stam1999,
	author = {Stam, Jos},
	title = {Stable Fluids},
	booktitle = {SIGGRAPH 99},
	year = {1999}
}
@article{foster1996,
	author = {Foster, Nick and Metaxas, Dimitris},
	title = {Realistic Animation of Liquids},
	journal = {Graphical Models and Image Processing},
	year = {1996},
	volume = {58},
	number = {5},
	pages = {471-483}
}
@article{enright2002,
	author = {Enright, Doug and Fedkiw, Ronald and Ferziger, Joel and Mitchell, Ian},
	title = {Animation and Rendering of Complex Water Surfaces},
	journal = {ACM Transactions on Graphics},
	year = {2002},
	volume = {21},
	number = {3},
	pages = {736-744}
}
@inproceedings{muller2002,
	author = {Müller, Matthias and Dorsey, Julie and McMillan, Leonard and Jagnow, Robert and Cutler, Barbara},
	title = {Stable Real-Time Deformations},
	booktitle = {ACM SIGGRAPH/Eurographics Symposium on Computer Animation},
	year = {2002}
}
```