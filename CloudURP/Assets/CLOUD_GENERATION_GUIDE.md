# 🌥️ Guia de Geração de Nuvens

## 🎯 Por Que a Nuvem Era Uma Esfera?

Antes, o código gerava **uma única esfera grande** no centro do volume. Agora você pode gerar **múltiplas nuvens** com formas e posições variadas!

---

## 🆕 Novos Parâmetros

### **Cloud Count** (1-10)
Quantidade de nuvens a serem geradas.
- **1-2**: Nuvem única ou par
- **3-5**: Grupo pequeno de nuvens
- **6-10**: Céu cheio de nuvens

### **Cloud Radius** (5-40)
Tamanho de cada esfera de nuvem (em células do grid).
- **5-10**: Nuvens pequenas e dispersas
- **15-20**: Nuvens médias (recomendado)
- **25-40**: Nuvens grandes e densas

### **Use Layered Clouds** (checkbox)
Muda o estilo de geração:
- **❌ Desativado**: Esferas aleatórias distribuídas
- **✅ Ativado**: Camadas achatadas (mais realista)

---

## 🎨 Configurações Recomendadas

### ☁️ Nuvens Isoladas (Cumulus)
```yaml
Cloud Count: 3
Cloud Radius: 18
Use Layered Clouds: ❌ Off
Grid Size: 128, 128, 128
Diffusion Rate: 0.06
```

### 🌫️ Camada de Nuvens (Stratus)
```yaml
Cloud Count: 4
Cloud Radius: 20
Use Layered Clouds: ✅ On
Grid Size: 128, 64, 128
Diffusion Rate: 0.04
```

### ☁️☁️☁️ Céu Nublado
```yaml
Cloud Count: 8
Cloud Radius: 12
Use Layered Clouds: ✅ On
Grid Size: 128, 96, 128
Diffusion Rate: 0.05
```

### 🌪️ Nuvem Única Densa
```yaml
Cloud Count: 1
Cloud Radius: 30
Use Layered Clouds: ❌ Off
Grid Size: 128, 128, 128
Diffusion Rate: 0.03
Source Scale: 2.0
```

---

## 🔍 Como Funciona

### **Modo Normal (Spheres)**
```
Para cada cloud:
  1. Gera posição aleatória (com viés para centro)
  2. Varia o tamanho (70% - 130% do cloud radius)
  3. Varia a densidade (80% - 120%)
  4. Injeta esfera com ruído procedural
```

**Características:**
- ✅ Distribuição 3D completa
- ✅ Tamanhos variados
- ✅ Mais volumétrico
- ⚠️ Pode parecer "bolhas" se diffusion baixo

### **Modo Layered**
```
Para cada cloud (2x o count):
  1. Gera posição em camada horizontal
  2. Y bias para altura média (25%-60% da altura)
  3. Nuvens mais achatadas (50% da altura)
  4. Maior espalhamento horizontal
```

**Características:**
- ✅ Mais realista (nuvens reais são achatadas)
- ✅ Forma camadas horizontais
- ✅ Melhor para céus cheios
- ⚠️ Menos variação vertical

---

## 🎲 Seed Aleatória

O sistema usa **seed fixa (42)** para gerar sempre as mesmas nuvens. Isso é útil para:
- ✅ Resultados consistentes
- ✅ Debug mais fácil
- ✅ Comparação de configurações

**Para mudar as nuvens:**
1. Mude `Cloud Count` ou `Cloud Radius`
2. Ou modifique o seed no código:
```csharp
System.Random rand = new System.Random(42); // Mude o número
```

---

## 💡 Dicas para Nuvens Interessantes

### 1. **Combine com Diffusion**
```
Alto Cloud Count + Alto Diffusion Rate = Névoa suave
Baixo Cloud Count + Baixo Diffusion Rate = Nuvens definidas
```

### 2. **Ajuste a Escala do Volume**
```
Scale Alto em Y = Nuvens mais verticais (cumulus)
Scale Baixo em Y = Nuvens achatadas (stratus)
```

### 3. **Use Source Scale**
```
Source Scale alto (3-5) = Nuvens mais densas e persistentes
Source Scale baixo (1-2) = Nuvens mais leves
```

### 4. **Experimente com Velocidade Inicial**
```
initialVelocity (0, 0.3, 0) = Nuvens subindo
initialVelocity (0.5, 0.1, 0) = Nuvens movendo horizontalmente
initialVelocity (0, -0.1, 0) = Nuvens descendo (chuva?)
```

### 5. **Injete Mais Densidade com SPACE**
Mesmo com a fonte constante, você pode:
- Segurar SPACE para adicionar mais densidade
- Criar "tempestades" localizadas
- Fazer nuvens crescerem organicamente

---

## 🔧 Ajustes Avançados

### Para Nuvens Mais Orgânicas:
1. Aumente **Diffusion Iterations** (20-30)
2. Use **Cloud Count alto** (7-10)
3. Ative **Use Layered Clouds**
4. Aguarde alguns segundos para diffusion agir

### Para Nuvens Mais Definidas:
1. Diminua **Diffusion Rate** (0.02-0.04)
2. Diminua **Diffusion Iterations** (10-15)
3. Desative **Use Layered Clouds**
4. Use **Cloud Radius menor** (10-15)

### Para Névoa Densa:
1. Use **Cloud Count muito alto** (10)
2. Ative **Use Layered Clouds**
3. **Cloud Radius grande** (25-35)
4. **Diffusion Rate alto** (0.1-0.15)
5. **Absorption baixo** (0.3-0.4)

---

## 📊 Impacto na Performance

A geração de nuvens acontece **uma vez no início** (Awake), então não afeta FPS durante gameplay.

**Tempo de geração:**
- Cloud Count 1-3: Instantâneo
- Cloud Count 4-6: < 0.1s
- Cloud Count 7-10: < 0.2s

**Memória:**
- Cada esfera adicional: ~insignificante
- Limitado pelo Grid Size (maior impacto)

---

## 🎓 Explicação Técnica

### O Que Mudou:

**Antes:**
```csharp
// Uma esfera gigante no centro
injectPos = (gridSize / 2, gridSize / 2, gridSize / 2)
injectRadius = min(gridSize) * 0.25  // 25% do volume!
```

**Agora:**
```csharp
// Loop gerando múltiplas esferas
for (i = 0; i < cloudCount; i++) {
    injectPos = random_within_bounds()
    injectRadius = cloudRadius * random(0.7, 1.3)
    injectValue = random(0.8, 1.2)
    // Injeta com ruído FBM do compute shader
}
```

### Ruído Procedural:
O compute shader (`InjectKernel`) já aplica **FBM (Fractal Brownian Motion)** a cada esfera, criando bordas irregulares e formas orgânicas.

---

## 🐛 Solução de Problemas

**Ainda parece uma esfera grande?**
- ✅ Aumente `Cloud Count` para 5+
- ✅ Ative `Use Layered Clouds`
- ✅ Espere alguns segundos para diffusion agir
- ✅ Verifique se `Diffusion Rate` > 0

**Nuvens desaparecendo?**
- ✅ Aumente `Source Scale` (3-5)
- ✅ Diminua `Diffusion Rate`
- ✅ Aumente `Inject Value`

**Nuvens muito pequenas?**
- ✅ Aumente `Cloud Radius` (20-30)
- ✅ Aumente `Cloud Count`
- ✅ Verifique a escala do Cube (Scale Y deve ser >= 40)

**Tudo branco/muito denso?**
- ✅ Diminua `Source Scale` (1-2)
- ✅ Diminua `Cloud Count`
- ✅ Aumente `Absorption` no rendering

---

Divirta-se criando diferentes formações de nuvens! ☁️🎨
