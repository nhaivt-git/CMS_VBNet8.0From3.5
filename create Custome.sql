
SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


CREATE TABLE `customers` (
  `customer_id` int(11) NOT NULL,
  `customer_name` varchar(255) NOT NULL,
  `customer_address` varchar(255) NOT NULL,
  `customer_phone` varchar(15) NOT NULL,
  `customer_email` varchar(45) NOT NULL,
  `customer_gender` varchar(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


INSERT INTO `customers` (`customer_id`, `customer_name`, `customer_address`, `customer_phone`, `customer_email`, `customer_gender`) VALUES
(3, 'anh', '123123', '13123123', 'a@gmail.com', 'Nữ'),
(5, 'anh3', '123123', '13123123', 'a@gmail.com', 'Nam');


ALTER TABLE `customers`
  ADD PRIMARY KEY (`customer_id`);


ALTER TABLE `customers`
  MODIFY `customer_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;
COMMIT;
