# CqlSharp
CQL (CSV Query Language) for C#
## Features

### From절의 데이터 소스로 csv 파일을 지정할 수 있습니다
```sql
SELECT * FROM "test0.csv"
```
<img width="469" alt="image" src="https://user-images.githubusercontent.com/50487467/209337002-d5c33303-e4a4-41ff-b38a-284197faf29b.png">

```sql
SELECT * FROM "test0.csv" AS csv
```
<img width="571" alt="image" src="https://user-images.githubusercontent.com/50487467/209337162-03c3fddd-bf00-4227-825f-af105e53148f.png">


### Select절에 컬럼명을 지정할 수 있습니다

```sql
SELECT firstname FROM "test0.csv"
```
<img width="453" alt="image" src="https://user-images.githubusercontent.com/50487467/209337360-127baaf8-bb2d-4d71-a6d4-a526af628b56.png">

### COUNT(*) 구문을 사용할 수 있습니다.

```sql
SELECT COUNT(*) FROM "test2.csv"
```
<img width="445" alt="image" src="https://user-images.githubusercontent.com/50487467/209337521-c9c7ccb2-afad-4086-9854-4ea6125b70b7.png">

```sql
SELECT COUNT(*) FROM "test2.csv" WHERE firstname LIKE 'e%'
```
<img width="618" alt="image" src="https://user-images.githubusercontent.com/50487467/209338191-6042ad2b-71aa-494b-a31f-e418c5032d75.png">


### Limit, Offset 구문을 사용할 수 있습니다.

```sql
SELECT * FROM "test1.csv" LIMIT 10 OFFSET 3
```
<img width="528" alt="image" src="https://user-images.githubusercontent.com/50487467/209337555-eabff4bb-9f0c-4d59-8d98-eefbaf3769d8.png">


### Where절을 사용할 수 있습니다.

```sql
SELECT * FROM "test1.csv" WHERE firstname = 'evan' AND lastname = 'choi'
```
<img width="728" alt="image" src="https://user-images.githubusercontent.com/50487467/209337747-ad04a457-d43a-46f6-81f5-4f84150e42bc.png">

```sql
SELECT * FROM "test1.csv" WHERE firstname REGEXP '^[Tt]on'
```
<img width="616" alt="image" src="https://user-images.githubusercontent.com/50487467/209338700-a1028207-d41e-4d67-b6dc-64826e621907.png">


### 간단한 표현식을 사용할 수 있습니다.

```sql
SELECT 'Hello, ' + 'World!!'
```
<img width="421" alt="image" src="https://user-images.githubusercontent.com/50487467/209337809-58cff689-e21e-4ba4-b114-bb48d3a915d8.png">

```sql
SELECT 1 + 1;
```
<img width="324" alt="image" src="https://user-images.githubusercontent.com/50487467/209337851-ae84f078-2666-4c56-95cb-8c4765462e8c.png">

### 서브쿼리를 지원합니다.

```sql
SELECT csv.name FROM (SELECT firstname AS name FROM "test0.csv") csv
```
<img width="722" alt="image" src="https://user-images.githubusercontent.com/50487467/209339161-0538db3b-d559-47d8-b967-0406e06deaaa.png">


### 쿼리 최적화를 지원합니다.
```sql
SELECT COUNT(*) FROM "test2.csv" WHERE firstname = 'evan' OR TRUE;
```

Not Optimized: `55ms` <br>
<img width="682" alt="image" src="https://user-images.githubusercontent.com/50487467/209339599-347f6487-dc2b-4f17-a9fe-422f0f1e15c4.png">

Optimized: `33ms` <br>
<img width="678" alt="image" src="https://user-images.githubusercontent.com/50487467/209339656-f8b2e6f1-68b9-4a7e-a770-311fbe37f78f.png">




