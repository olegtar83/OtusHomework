# Индексы

1. Проверочный запрос 
    ```
    EXPLAIN ANALYSE
    SELECT id, first_name, second_name, sex, age, city, biography
    FROM public."user"
    WHERE first_name LIKE 'Ива%' AND second_name LIKE 'Т%'
    ```
2. Запрос без индекса:
    ``` Gather  (cost=1000.00..22521.61 rows=766 width=90) (actual time=904.236..2454.411 rows=700 loops=1)
       Workers Planned: 2
       Workers Launched: 2
       ->  Parallel Seq Scan on "user"  (cost=0.00..21445.01 rows=319 width=90) (actual time=881.520..2425.928 rows=233 loops=3)
             Filter: (((first_name)::text ~~ 'Ива%'::text) AND ((second_name)::text ~~ 'Т%'::text))
             Rows Removed by Filter: 333100
     Planning Time: 36.957 ms
     Execution Time: 2454.595 ms```
3. Создаем индекс: 
    ```
     CREATE INDEX user_full_data_idx ON public."user" using btree (first_name text_pattern_ops,second_name text_pattern_ops) INCLUDE (id, sex, age, city, biography) ;
    ```
4. Запрос с индексом:
     ```
    Index Only Scan using user_full_data_idx on "user"  (cost=0.55..1792.85 rows=684 width=90) (actual time=44.612..81.359 rows=700 loops=1)
       Index Cond: ((first_name ~>=~ 'Ива'::text) AND (first_name ~<~ 'Ивб'::text) AND (second_name ~>=~ 'Т'::text) AND (second_name ~<~ 'У'::text))
       Filter: (((first_name)::text ~~ 'Ива%'::text) AND ((second_name)::text ~~ 'Т%'::text))
       Heap Fetches: 0
     Planning Time: 1.626 ms
     Execution Time: 81.467 ms
     ```
 5. Покрывающий индекс лучше обычного, потому что когда мы выполняем запрос, который ищет по first_name и last_name, PostgreSQL может извлечь остальные поля сразу из 
    индекса без необходимости дополнительного доступа к таблице.

 6. Отчеты связанные с тестировкой нагруженности находятся в папке.  

