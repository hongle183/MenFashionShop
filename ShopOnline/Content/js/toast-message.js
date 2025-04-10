function showToast({ type = '', message = '' }) {
    const toast = document.querySelector('#alert-toast');
    if (toast) {
        const icons = {
            success: '<i class="fa-solid fa-circle-check"></i>',
            info: '<i class="fa-solid fa-circle-info"></i>',
            danger: '<i class="fa-solid fa-circle-exclamation"></i>'
        };
        const titles = {
            success: 'Thành công',
            info: 'Thông báo',
            danger: 'Không thành công'
        };

        const content = document.createElement('div');
        content.classList.add('alert', `alert-${type}`)
        content.innerHTML = `
                    <div class="alert-icon">
                        ${icons[type]}
                    </div>            
                    <div class="alert-content">
                        <h5>${titles[type]}</h5>
                        <p>${message}</p>
                    </div>
                    <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                `;

        toast.appendChild(content);

        const autorRemoveId = setTimeout(() => {
            toast.removeChild(content);
        }, 4500);
        content.onclick = (e) => {
            if (e.target.closest('.close')) {
                toast.removeChild(content);
                clearTimeout(autorRemoveId);
            }
        }
    }
}