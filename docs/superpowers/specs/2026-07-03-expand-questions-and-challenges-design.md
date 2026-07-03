# Design Spec: Expand Questions and Challenges in Grit Roulette

Expand the collection of questions and challenges for both the Spin Bottle and Secret Vote mini-games. This additions focus on romantic history, ex-partners (nyc), and lighthearted, gentle intimate/NSFW topics to make the social party game more engaging and diverse.

## Proposed Changes

### 1. Spin Bottle Challenges
We will expand the hardcoded `challenges` array in [SpinBottleGameManager.cs](file:///Users/admin/Projects/2026/grit-roulette/Assets/_Game/Scripts/Games/SpinBottleGameManager.cs) to contain 51 total challenges (5 existing + 46 new).

### 2. Secret Vote Questions
We will expand the hardcoded `questions` array in [SecretVoteGameManager.cs](file:///Users/admin/Projects/2026/grit-roulette/Assets/_Game/Scripts/Games/SecretVoteGameManager.cs) to contain 51 total questions (5 existing + 46 new).

---

## Detailed Lists

### Challenges (`SpinBottleGameManager.cs`)
1. "Uống 1 hớp cùng người đối diện!" (Sẵn có)
2. "Kể một bí mật nhậu nhẹt đáng xấu hổ nhất của bạn." (Sẵn có)
3. "Thực hiện 5 cái chống đẩy hoặc uống 1 hớp." (Sẵn có)
4. "Uống 2 hớp hoặc cho người bên trái véo má." (Sẵn có)
5. "Chỉ định một người khác uống cùng bạn vòng này." (Sẵn có)
6. "Uống 2 hớp hoặc kể tên người yêu cũ gần đây nhất của bạn."
7. "Kể một thói quen xấu của người yêu cũ mà bạn từng ghét nhất, hoặc uống 2 hớp."
8. "Uống 1 hớp nếu bạn từng nhắn tin cho người yêu cũ lúc say."
9. "Uống 1 hớp hoặc trả lời: Bạn thích ôm từ phía trước hay phía sau hơn khi ngủ?"
10. "Kể tên bộ phận quyến rũ nhất của người bên cạnh khiến bạn chú ý, hoặc uống 1 hớp."
11. "Uống 2 hớp hoặc trả lời thật lòng: Bạn đã bao giờ làm \"chuyện ấy\" ở một nơi không phải giường ngủ chưa?"
12. "Cho cả bàn xem tin nhắn gần nhất của bạn với người yêu cũ (nếu còn giữ), hoặc uống 3 hớp."
13. "Kể về một kỷ niệm hài hước/trớ trêu nhất xảy ra khi đang hẹn hò lãng mạn."
14. "Uống 1 hớp nếu bạn từng giả vờ ngủ để tránh \"làm chuyện ấy\"."
15. "Thực hiện một điệu nhảy quyến rũ (sexy dance) trong 10 giây hoặc uống 2 hớp."
16. "Nhắm mắt lại, sờ tay đoán xem người bên cạnh là ai, nếu đoán sai uống 2 hớp."
17. "Uống 2 hớp hoặc cho người bên phải xem hình ảnh/album ảnh riêng tư nhất của bạn."
18. "Thú nhận một điều bạn từng làm lén lút sau lưng người yêu cũ nhưng chưa bao giờ kể."
19. "Kể về buổi hẹn hò tồi tệ nhất bạn từng trải qua."
20. "Bạn có từng \"vượt rào\" trong buổi hẹn hò đầu tiên không? Trả lời thật lòng hoặc uống 2 hớp."
21. "Uống 1 hớp nếu bạn vẫn còn giữ quà lưu niệm của bất kỳ người yêu cũ nào."
22. "Nhắn tin cho người yêu cũ câu: \"Hôm nay thời tiết đẹp nhỉ\" hoặc uống 3 hớp."
23. "Kể về gu thời trang phòng ngủ của bạn: bạn thích mặc gì nhất khi đi ngủ?"
24. "Kể một điều lãng mạn nhất bạn từng làm cho một người, hoặc uống 2 hớp."
25. "Uống 2 hớp hoặc thực hiện thử thách: Cho người khác chọn một người ở đây để bạn gửi một lời khen ngọt ngào nhất."
26. "Bạn đã từng hẹn hò với hai người cùng một lúc chưa? Trả lời hoặc uống 2 hớp."
27. "Uống 1 hớp và tiết lộ: Bạn thích nụ hôn kiểu Pháp hay nụ hôn nhẹ nhàng lên trán hơn?"
28. "Kể về nụ hôn đầu tiên của bạn: diễn ra ở đâu và cảm xúc lúc đó thế nào?"
29. "Uống 2 hớp hoặc kể tên một người trong bàn này mà bạn có ấn tượng đầu tiên tốt nhất."
30. "Cho phép người đối diện đặt một câu hỏi bất kỳ về đời sống tình cảm của bạn, bạn phải trả lời thật lòng hoặc uống 3 hớp."
31. "Kể về lời nói dối ngớ ngẩn nhất bạn từng dùng để chia tay hoặc từ chối ai đó."
32. "Uống 1 hớp nếu bạn từng khóc vì người yêu cũ trong vòng 6 tháng qua."
33. "Kể về tư thế ngủ kỳ lạ nhất của bạn khi ngủ chung giường với người khác."
34. "Bạn nghĩ gì về mối quan hệ \"Friends with benefits\" (FWB)? Trả lời thật lòng hoặc uống 2 hớp."
35. "Uống 2 hớp hoặc thú nhận: Bạn từng có suy nghĩ lãng mạn nào với bất cứ ai đang ngồi trong bàn này không?"
36. "Gửi một tin nhắn thoại thì thầm vào tai người bên cạnh hoặc uống 1 hớp."
37. "Kể về trải nghiệm lãng mạn ngượng ngùng nhất của bạn, hoặc uống 2 hớp."
38. "Uống 1 hớp nếu bạn từng kiểm tra điện thoại của người yêu cũ mà không để họ biết."
39. "Kể về điều điên rồ nhất bạn từng làm vì tình yêu."
40. "Hãy cho người bên cạnh cù lét trong 10 giây mà không được cười, nếu cười thì uống 1 hớp."
41. "Trả lời thật lòng: Bạn có tin vào tình yêu sét đánh không? Không trả lời uống 1 hớp."
42. "Uống 2 hớp hoặc tiết lộ tần suất lý tưởng cho chuyện phòng ngủ của bạn trong một tuần."
43. "Nếu được quay lại thời gian, bạn có muốn thay đổi bất cứ điều gì về mối tình đầu của mình không?"
44. "Uống 1 hớp nếu bạn từng bấm nhầm nút thích/tim ảnh rất cũ của nyc khi đang \"stalk\"."
45. "Kể về một nơi kỳ lạ nhất mà bạn từng nảy sinh ham muốn lãng mạn."
46. "Trả lời thật lòng: Bạn thích ánh sáng đèn mờ ảo hay tắt đèn hoàn toàn khi lãng mạn?"
47. "Uống 2 hớp hoặc chia sẻ suy nghĩ thật lòng nhất của bạn về người yêu cũ của người bên trái."
48. "Nói một câu bằng giọng điệu quyến rũ/sexy nhất hướng về phía người đối diện hoặc uống 2 hớp."
49. "Uống 1 hớp nếu bạn từng đồng ý hẹn hò chỉ vì cô đơn chứ không thực sự thích người đó."
50. "Chia sẻ một điểm yếu hoặc \"nút nhạy cảm\" trên cơ thể của bạn (ví dụ: cổ, tai...) khi được chạm vào."
51. "Uống 2 hớp hoặc kể về lần bạn say xỉn và làm chuyện ngớ ngẩn nhất trước mặt người mình thích."

### Questions (`SecretVoteGameManager.cs`)
1. "Ai dễ say nhất?" (Sẵn có)
2. "Ai hay đến muộn nhất?" (Sẵn có)
3. "Ai hay quên nhất?" (Sẵn có)
4. "Ai là người nói nhiều nhất?" (Sẵn có)
5. "Ai nên uống vòng này nhất?" (Sẵn có)
6. "Ai là người lụy người yêu cũ nhất?"
7. "Ai là người có nhiều người yêu cũ nhất ở đây?"
8. "Ai có vẻ ngoài ngây thơ nhất nhưng \"quái vật\" ngầm khi yêu?"
9. "Ai dễ đồng ý quay lại với người yêu cũ nhất nếu được rủ rê?"
10. "Ai dễ nảy sinh tình cảm/say nắng nhất khi say?"
11. "Ai hay xem phim \"hấp dẫn\" nhiều nhất?"
12. "Ai là người giữ nhiều bí mật thầm kín nhất?"
13. "Ai là người lãng mạn (hoặc sến sẩm) nhất khi yêu?"
14. "Ai dễ có xu hướng \"nhìn trộm\" điện thoại của người yêu nhất?"
15. "Ai có gu chọn người yêu độc lạ nhất?"
16. "Ai dễ bị thu hút bởi người lớn tuổi hơn mình nhiều nhất?"
17. "Ai là người có khả năng nhắn tin thả thính cùng lúc nhiều người nhất?"
18. "Ai dễ \"cảm nắng\" bạn thân nhất?"
19. "Ai là người giữ thể diện nhất trước mặt người yêu cũ?"
20. "Ai dễ tin vào những lời thề non hẹn biển nhất?"
21. "Ai là người thích kiểm soát người yêu nhất?"
22. "Ai dễ quên sinh nhật hay ngày kỷ niệm của người yêu nhất?"
23. "Ai là người hay ghen tuông vô cớ nhất?"
24. "Ai dễ bị dụ dỗ bằng đồ ăn hoặc một chầu nhậu nhất?"
25. "Ai có khả năng giấu chuyện mình đang yêu giỏi nhất?"
26. "Ai là người hay stalk trang cá nhân của nyc nhiều nhất?"
27. "Ai dễ đồng ý đi du lịch riêng với người mới quen nhất?"
28. "Ai là người chi tiêu hào phóng nhất khi hẹn hò?"
29. "Ai dễ giận dỗi người yêu vì những lý do vô lý nhất?"
30. "Ai là người giỏi che giấu cảm xúc khi buồn nhất?"
31. "Ai có khả năng trở thành \"quân sư tình yêu\" giỏi nhất cho người khác?"
32. "Ai dễ yêu xa giỏi nhất mà không sợ cô đơn?"
33. "Ai là người hay có những phát ngôn \"triết lý tình yêu\" sến sẩm nhất?"
34. "Ai dễ nói lời chia tay trước nhất khi có mâu thuẫn nhỏ?"
35. "Ai là người thích được người yêu nuông chiều như em bé nhất?"
36. "Ai có khả năng say xỉn xong gọi điện khóc lóc với nyc cao nhất?"
37. "Ai dễ tha thứ cho sự lừa dối trong tình yêu nhất?"
38. "Ai có nụ cười tỏa nắng/thu hút người khác phái nhất ở đây?"
39. "Ai dễ bị thu hút bởi ngoại hình hơn là tính cách nhất?"
40. "Ai là người kén chọn nhất trong việc tìm kiếm bạn đời?"
41. "Ai dễ yêu từ cái nhìn đầu tiên nhất?"
42. "Ai là người hay thả thính bằng những câu thơ sến nhất?"
43. "Ai dễ có biểu cảm \"mê gái/mê trai\" lộ liễu nhất khi gặp người đẹp?"
44. "Ai là người thích những trò đùa tinh nghịch/quyến rũ nhẹ nhàng nhất?"
45. "Ai có khả năng ngủ quên luôn trong lúc đang nhắn tin lãng mạn?"
46. "Ai dễ giận dỗi nhưng cũng dễ dỗ dành nhất?"
47. "Ai là người thích gây bất ngờ cho nửa kia bằng những món quà độc lạ nhất?"
48. "Ai dễ bị đỏ mặt nhất khi được ai đó thì thầm vào tai?"
49. "Ai có phong thái tự tin nhất khi đi tán tỉnh người khác?"
50. "Ai là người thích sờ tóc hoặc nắm tay người yêu mọi lúc mọi nơi nhất?"
51. "Ai dễ đồng ý tham gia một trò chơi thử thách mạo hiểm/táo bạo nhất?"
