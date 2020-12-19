### Разделитель пакетов PL/SQL

- Автоматическое разделение пакета PL/SQL на основе полного лексического анализа кода с использованием ANTLR4.
- В исходном пакете необходимо отметить методы и переменные, которые должны отправиться в новый пакет.
- Особенность приложения в том, что оно автоматически найдёт и исправит отсутствующие связи между новым и исходным пакетом. (Альтернативная ручная работа была крайне неэффективна)
- Приложение применялось для разделения огромных пакетов c более чем 30К строк.

![](https://raw.githubusercontent.com/dbobylev/PackageSplitter/master/screen.png)