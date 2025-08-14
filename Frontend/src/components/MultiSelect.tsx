import React from 'react';
import Select, { type MultiValue, type ActionMeta, type StylesConfig } from 'react-select';

export interface OptionType {
  value: string;
  label: string;
}

interface MultiSelectProps {
  options: OptionType[];
  value: string[];
  onChange: (selectedValues: string[]) => void;
  placeholder?: string;
  isLoading?: boolean;
  isMulti?: boolean;
  closeMenuOnSelect?: boolean;
}

const MultiSelect: React.FC<MultiSelectProps> = ({
  options,
  value,
  onChange,
  placeholder = 'Выберите...',
  isLoading = false,
  isMulti = true,
  closeMenuOnSelect = false,
}) => {
  const handleChange = (
    newValue: MultiValue<OptionType>,
    actionMeta: ActionMeta<OptionType>
  ) => {
    if (isMulti) {
      const values = (newValue as OptionType[]).map(option => option.value);
      onChange(values);
    } else {
      const val = newValue ? [(newValue as unknown as OptionType).value] : [];
      onChange(val);
    }
  };

  const selectedOptions = options.filter(option => value.includes(option.value));

  return (
    <Select
      isMulti={isMulti}
      options={options}
      value={selectedOptions}
      onChange={handleChange}
      placeholder={placeholder}
      isLoading={isLoading}
      loadingMessage={() => "Загрузка..."}
      noOptionsMessage={() => "Нет доступных вариантов"}
      className="w-full"
      classNamePrefix="select"
      closeMenuOnSelect={closeMenuOnSelect}
    />
  );
};

export default MultiSelect;