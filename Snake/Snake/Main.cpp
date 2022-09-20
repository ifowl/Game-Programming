#include <SFML/Graphics.hpp>
#include <time.h>
#include <iostream>
#include <sstream>

using namespace sf;

int N = 30, M = 20;
int size = 16;
int w = size * N;
int h = size * M;

int dir, num = 4;

bool gameOver = false;

struct Snake
{
    int x, y;
}  s[100];

struct Fruit
{
    int x, y;
} f;

void Tick()
{
    for (int i = num; i > 0; --i)
    {
        s[i].x = s[i - 1].x; s[i].y = s[i - 1].y;
    }

    if (dir == 0) s[0].y += 1;
    if (dir == 1) s[0].x -= 1;
    if (dir == 2) s[0].x += 1;
    if (dir == 3) s[0].y -= 1;

    if ((s[0].x == f.x) && (s[0].y == f.y))
    {
        num++; f.x = rand() % N; f.y = rand() % M;
    }

    if (s[0].x > N) s[0].x = 0;  if (s[0].x < 0) s[0].x = N;
    if (s[0].y > M) s[0].y = 0;  if (s[0].y < 0) s[0].y = M;

    for (int i = 1; i < num; i++)
        if (s[0].x == s[i].x && s[0].y == s[i].y) {
            gameOver = true;
        } //num = i;
}

int main()
{
    srand(time(0));

    RenderWindow window(VideoMode(w, h), "Snake");

    Texture t1, t2, t3;
    t1.loadFromFile("images/white.png");
    t2.loadFromFile("images/red.png");
    t3.loadFromFile("images/green.png");

    Sprite sprite1(t1);
    Sprite sprite2(t2);
    Sprite sprite3(t3);

    //Text for game over screen
    Font font;
    font.loadFromFile("images/arcade.ttf");
    
    Text endGame;
    endGame.setCharacterSize(50);
    endGame.setPosition(120, 100);
    endGame.setFont(font);
    endGame.setFillColor(Color::Red);
    endGame.setString("GAME OVER");
    Text restart;
    restart.setCharacterSize(15);
    restart.setPosition(130, 240);
    restart.setFont(font);
    restart.setString("Press   ENTER   to   play   again");

    Clock clock;
    float timer = 0, delay = 0.08;

    f.x = 10;
    f.y = 10;

    while (window.isOpen())
    {
        float time = clock.getElapsedTime().asSeconds();
        clock.restart();
        timer += time;

        Event e;
        while (window.pollEvent(e))
        {
            if (e.type == Event::Closed)
                window.close();
        }

        if (Keyboard::isKeyPressed(Keyboard::Left)) dir = 1;
        if (Keyboard::isKeyPressed(Keyboard::Right)) dir = 2;
        if (Keyboard::isKeyPressed(Keyboard::Up)) dir = 3;
        if (Keyboard::isKeyPressed(Keyboard::Down)) dir = 0;

        if (timer > delay) { timer = 0; Tick(); }

        ////// draw  ///////
        window.clear();

        for (int i = 0; i < N; i++)
            for (int j = 0; j < M; j++)
            {
                sprite1.setPosition(i * size, j * size);  window.draw(sprite1);
            }

        for (int i = 0; i < num; i++)
        {
            sprite3.setPosition(s[i].x * size, s[i].y * size);  window.draw(sprite3);
        }

        sprite2.setPosition(f.x * size, f.y * size);  window.draw(sprite2);

        //game over screen triggered by boolean
        while (gameOver == true) {

            Event e;
            while (window.pollEvent(e))
            {
                if (e.type == Event::Closed)
                    window.close();
            }
            //pressing enter will restart the game.
            if (Keyboard::isKeyPressed(Keyboard::Return)) {
                gameOver = false;
                num = 4;
                break;
            }
            window.clear();
            window.draw(endGame);
            window.draw(restart);
            Text endScore;
            endScore.setCharacterSize(30);
            endScore.setPosition(180, 150);
            endScore.setFont(font);
            std::ostringstream finalScore;
            finalScore << "Score   " << num-4;

            endScore.setString(finalScore.str());
            window.draw(endScore);
            window.display();
        }

        window.display();
    }
    /*
    while (gameOver == true) {

        Event e;
        while (window.pollEvent(e))
        {
            if (e.type == Event::Closed)
                window.close();
        }
        window.clear();
        for (int i = 0; i < num; i++)
        {
            sprite3.setPosition(s[i].x * size, s[i].y * size);  window.draw(sprite3);
        }
        window.display();
    }
    */

    return 0;
}