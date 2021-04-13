import { DataType } from "~/models/Enums/DataType";

export const localize = (type: DataType) => {
  switch (type) {
    case DataType.Boolean:
      return "Тождественный"
    case DataType.String:
      return "Строковый"
    case DataType.Integer:
      return "Целочисленный"
    case DataType.Float:
      return "С плавающей запятой"
    case DataType.Date:
      return "Дата"
    case DataType.DateTime:
      return "Дата и время"
    case DataType.Time:
      return "Время"
    default:
      return "Неопределен"
  }
}
